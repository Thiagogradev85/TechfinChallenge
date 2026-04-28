using Microsoft.Extensions.Logging;
using TechfinChallenge.Clientes.Api.Services;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Clientes.Api.Messaging;

public class TransacaoEventHandler : ITransacaoEventHandler
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<TransacaoEventHandler> _logger;

    public TransacaoEventHandler(IClienteService clienteService, ILogger<TransacaoEventHandler> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    public Task HandleAsync(TransacaoAprovadaEvent evento)
    {
        _logger.LogInformation(
            "[EVENT HANDLER] Recebido evento | ClienteId: {ClienteId} | Tipo: {Tipo} | Valor: {Valor}",
            evento.ClienteId, evento.Tipo, evento.Valor);

        var cliente = _clienteService.BuscarPorId(evento.ClienteId);

        if (cliente == null)
        {
            _logger.LogWarning("[EVENT HANDLER] Cliente {ClienteId} não encontrado — evento ignorado.", evento.ClienteId);
            return Task.CompletedTask;
        }

        var limiteAnterior = cliente.ValorLimite;
        var novoLimite = evento.Tipo == "credito"
            ? cliente.ValorLimite + evento.Valor
            : cliente.ValorLimite - evento.Valor;

        _clienteService.AtualizarLimite(cliente.Id, novoLimite);

        _logger.LogInformation(
            "[EVENT HANDLER] Limite atualizado | Cliente: {Nome} | {LimiteAnterior} → {NovoLimite}",
            cliente.Nome, limiteAnterior, novoLimite);

        return Task.CompletedTask;
    }
}
