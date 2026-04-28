using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.Kafka;

public class AuditoriaConsumer : BackgroundService
{
    private readonly ILogger<AuditoriaConsumer> _logger;
    private const string TopicName = "transacao.aprovada";
    private const string GroupId = "auditoria-api";

    public AuditoriaConsumer(ILogger<AuditoriaConsumer> logger)
    {
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
            "[AUDITORIA] Inscrito no topic '{Topic}' | Group: {GroupId}",
            TopicName, GroupId);

        await Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    var evento = JsonSerializer.Deserialize<TransacaoAprovadaEvent>(result.Message.Value);

                    if (evento != null)
                    {
                        _logger.LogInformation(
                            "[AUDITORIA] *** REGISTRO DE AUDITORIA *** | ClienteId: {ClienteId} | Tipo: {Tipo} | Valor: {Valor} | Partition: {Partition} | Offset: {Offset}",
                            evento.ClienteId, evento.Tipo, evento.Valor,
                            result.Partition.Value, result.Offset.Value);
                    }

                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AUDITORIA] Erro ao processar mensagem — continuando...");
                }
            }

            consumer.Close();
            _logger.LogInformation("[AUDITORIA] Consumer encerrado.");
        }, stoppingToken);
    }
}
