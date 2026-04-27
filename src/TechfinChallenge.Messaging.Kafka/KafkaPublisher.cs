using Confluent.Kafka;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.Kafka;

public class KafkaPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private const string TopicName = "transacao.aprovada";

    public KafkaPublisher()
    {
        var config = new ProducerConfig { BootstrapServers = "localhost:29092" };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublicarAsync(TransacaoAprovadaEvent evento)
    {
        var json = JsonSerializer.Serialize(evento);
        await _producer.ProduceAsync(TopicName, new Message<string, string>
        {
            Key = evento.ClienteId,
            Value = json
        });
    }

    public void Dispose() => _producer.Dispose();
}
