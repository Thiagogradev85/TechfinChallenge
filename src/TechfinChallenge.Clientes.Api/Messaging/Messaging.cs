using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TechfinChallenge.Clientes.Api.Repositories;

namespace TechfinChallenge.Clientes.Api.Messaging;

public class TransacaoConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string FilaNome = "transacao.aprovada";

    public TransacaoConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(FilaNome, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensagem = Encoding.UTF8.GetString(body);
            var dados = JsonSerializer.Deserialize<TransacaoMensagem>(mensagem);

            if (dados != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ClienteRepository>();

                var cliente = repository.BuscarPorId(dados.ClienteId);
                if (cliente != null)
                {
                    var novoLimite = cliente.ValorLimite - dados.Valor;
                    repository.AtualizarLimite(cliente.Id, novoLimite);
                }
            }

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        await _channel!.BasicConsumeAsync(FilaNome, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}

public record TransacaoMensagem(string ClienteId, decimal Valor);
