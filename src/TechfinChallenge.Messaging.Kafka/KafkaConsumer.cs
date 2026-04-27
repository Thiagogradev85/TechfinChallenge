using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.Kafka;

public class KafkaConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private const string TopicName = "transacao.aprovada";
    private const string GroupId = "clientes-api";

    public KafkaConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
                        using var scope = _serviceProvider.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<ITransacaoEventHandler>();
                        handler.HandleAsync(evento).GetAwaiter().GetResult();
                    }

                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // continua consumindo; erro individual não para o consumer
                }
            }

            consumer.Close();
        }, stoppingToken);
    }
}
