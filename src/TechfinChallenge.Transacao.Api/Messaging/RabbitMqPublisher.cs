using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Modelo = TechfinChallenge.Transacao.Api.Models.TransacaoModel;

namespace TechfinChallenge.Transacao.Api.Messaging;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string FilaNome = "transacao.aprovada";

    public RabbitMqPublisher()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.QueueDeclareAsync(FilaNome, durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
    }

    public void Publicar(Modelo transacao)
    {
        var mensagem = JsonSerializer.Serialize(new
        {
            transacao.ClienteId,
            transacao.Valor
        });

        var body = Encoding.UTF8.GetBytes(mensagem);

        _channel.BasicPublishAsync(
            exchange: "",
            routingKey: FilaNome,
            body: body).GetAwaiter().GetResult();
    }
}
