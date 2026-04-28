using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.Kafka;

public class KafkaConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaConsumer> _logger;
    private const string TopicName = "transacao.aprovada";
    private const string DlqTopicName = "transacao.falha";
    private const string GroupId = "clientes-api";
    private const int MaxRetries = 3;

    public KafkaConsumer(IServiceProvider serviceProvider, ILogger<KafkaConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:29092",
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var dlqProducer = new ProducerBuilder<string, string>(
            new ProducerConfig { BootstrapServers = "localhost:29092" }).Build();

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(TopicName);

        _logger.LogInformation(
            "[KAFKA CONSUMER] Inscrito no topic '{Topic}' | Group: {GroupId}",
            TopicName, GroupId);

        await Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    _logger.LogInformation(
                        "[KAFKA CONSUMER] Mensagem recebida | Partition: {Partition} | Offset: {Offset} | Key: {Key}",
                        result.Partition.Value, result.Offset.Value, result.Message.Key);

                    var evento = JsonSerializer.Deserialize<TransacaoAprovadaEvent>(result.Message.Value);

                    if (evento != null)
                        ProcessWithRetry(evento, result, consumer, dlqProducer);
                    else
                    {
                        consumer.Commit(result);
                        _logger.LogWarning("[KAFKA CONSUMER] Mensagem inválida (null) — offset commitado sem processar.");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[KAFKA CONSUMER] Erro inesperado no loop principal.");
                }
            }

            consumer.Close();
            _logger.LogInformation("[KAFKA CONSUMER] Consumer encerrado.");
        }, stoppingToken);
    }

    // internal para permitir testes unitários via InternalsVisibleTo
    internal void ProcessWithRetry(
        TransacaoAprovadaEvent evento,
        ConsumeResult<string, string> result,
        IConsumer<string, string> consumer,
        IProducer<string, string> dlqProducer,
        TimeSpan? retryDelay = null)
    {
        Exception? lastEx = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation(
                    "[KAFKA CONSUMER] Tentativa {Attempt}/{MaxRetries} | ClienteId: {ClienteId} | Tipo: {Tipo} | Valor: {Valor}",
                    attempt, MaxRetries, evento.ClienteId, evento.Tipo, evento.Valor);

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ITransacaoEventHandler>();
                handler.HandleAsync(evento).GetAwaiter().GetResult();

                // Sucesso — commita o offset e sai
                consumer.Commit(result);
                _logger.LogInformation(
                    "[KAFKA CONSUMER] Offset commitado | Partition: {Partition} | Offset: {Offset}",
                    result.Partition.Value, result.Offset.Value);
                return;
            }
            catch (Exception ex)
            {
                lastEx = ex;
                _logger.LogWarning(
                    "[KAFKA CONSUMER] Tentativa {Attempt}/{MaxRetries} falhou | ClienteId: {ClienteId} | Erro: {Message}",
                    attempt, MaxRetries, evento.ClienteId, ex.Message);

                // Backoff progressivo (injetável para testes usarem TimeSpan.Zero)
                if (attempt < MaxRetries)
                    Thread.Sleep(retryDelay ?? TimeSpan.FromSeconds(attempt));
            }
        }

        // Todas as tentativas esgotadas → manda para a DLQ
        _logger.LogError(lastEx,
            "[KAFKA CONSUMER] {MaxRetries} tentativas esgotadas | ClienteId: {ClienteId} → DLQ '{DlqTopic}'",
            MaxRetries, evento.ClienteId, DlqTopicName);

        var dlqPayload = JsonSerializer.Serialize(new
        {
            evento,
            erro = lastEx?.Message,
            tentativas = MaxRetries,
            timestamp = DateTime.UtcNow
        });

        dlqProducer.Produce(DlqTopicName, new Message<string, string>
        {
            Key = evento.ClienteId,
            Value = dlqPayload
        });
        dlqProducer.Flush(TimeSpan.FromSeconds(5));

        _logger.LogWarning(
            "[KAFKA CONSUMER] Evento enviado para DLQ '{DlqTopic}' | ClienteId: {ClienteId}",
            DlqTopicName, evento.ClienteId);

        // Commita o offset mesmo após DLQ — partição não pode ficar bloqueada
        consumer.Commit(result);
        _logger.LogInformation(
            "[KAFKA CONSUMER] Offset commitado após DLQ | Partition: {Partition} | Offset: {Offset}",
            result.Partition.Value, result.Offset.Value);
    }
}
