using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.Kafka;

public class KafkaPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaPublisher> _logger;
    private const string TopicName = "transacao.aprovada";

    public KafkaPublisher(ILogger<KafkaPublisher> logger)
    {
        _logger = logger;
        var config = new ProducerConfig { BootstrapServers = "localhost:29092" };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublicarAsync(TransacaoAprovadaEvent evento)
    {
        var json = JsonSerializer.Serialize(evento);

        _logger.LogInformation(
            "[KAFKA PRODUCER] Publicando no topic '{Topic}' | Key: {ClienteId} | Tipo: {Tipo} | Valor: {Valor}",
            TopicName, evento.ClienteId, evento.Tipo, evento.Valor);

        var result = await _producer.ProduceAsync(TopicName, new Message<string, string>
        {
            Key = evento.ClienteId,
            Value = json
        });

        _logger.LogInformation(
            "[KAFKA PRODUCER] Mensagem entregue | Partition: {Partition} | Offset: {Offset}",
            result.Partition.Value, result.Offset.Value);
    }

    public void Dispose() => _producer.Dispose();
}
