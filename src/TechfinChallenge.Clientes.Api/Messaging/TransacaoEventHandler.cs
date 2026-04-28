using TechfinChallenge.Clientes.Api.Services;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Clientes.Api.Messaging;

public class TransacaoEventHandler : ITransacaoEventHandler
{
    private readonly IClienteService _clienteService;

    public TransacaoEventHandler(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public Task HandleAsync(TransacaoAprovadaEvent evento)
    {
        var cliente = _clienteService.BuscarPorId(evento.ClienteId);
        if (cliente == null) return Task.CompletedTask;

        var novoLimite = evento.Tipo == "credito"
            ? cliente.ValorLimite + evento.Valor
            : cliente.ValorLimite - evento.Valor;

        _clienteService.AtualizarLimite(cliente.Id, novoLimite);
        return Task.CompletedTask;
    }
}
