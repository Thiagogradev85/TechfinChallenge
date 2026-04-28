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
    private const string GroupId = "clientes-api";

    public KafkaConsumer(IServiceProvider serviceProvider, ILogger<KafkaConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:29092",
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
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
                    {
                        _logger.LogInformation(
                            "[KAFKA CONSUMER] Processando evento | ClienteId: {ClienteId} | Tipo: {Tipo} | Valor: {Valor}",
                            evento.ClienteId, evento.Tipo, evento.Valor);

                        using var scope = _serviceProvider.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<ITransacaoEventHandler>();
                        handler.HandleAsync(evento).GetAwaiter().GetResult();
                    }

                    consumer.Commit(result);

                    _logger.LogInformation(
                        "[KAFKA CONSUMER] Offset commitado | Partition: {Partition} | Offset: {Offset}",
                        result.Partition.Value, result.Offset.Value);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[KAFKA CONSUMER] Erro ao processar mensagem — continuando...");
                }
            }

            consumer.Close();
            _logger.LogInformation("[KAFKA CONSUMER] Consumer encerrado.");
        }, stoppingToken);
    }
}
