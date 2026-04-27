using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.RabbitMQ;

public class RabbitMqPublisher : IEventPublisher, IHostedService, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private const string FilaNome = "transacao.aprovada";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(FilaNome, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
    }

    public async Task PublicarAsync(TransacaoAprovadaEvent evento)
    {
        var json = JsonSerializer.Serialize(evento);
        var body = Encoding.UTF8.GetBytes(json);
        await _channel!.BasicPublishAsync(exchange: "", routingKey: FilaNome, body: body);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }
}
