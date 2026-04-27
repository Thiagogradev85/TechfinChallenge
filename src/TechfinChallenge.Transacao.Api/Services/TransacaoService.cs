using System.Net.Http.Json;
using TechfinChallenge.Messaging.Abstractions;
using TechfinChallenge.Transacao.Api.DTOs;
using TechfinChallenge.Transacao.Api.Models;
using TechfinChallenge.Transacao.Api.Repositories;
using TechfinChallenge.Shared;

namespace TechfinChallenge.Transacao.Api.Services;

public class TransacaoService : ITransacaoService
{
    private readonly ITransacaoRepository _repository;
    private readonly IEventPublisher _publisher;
    private readonly HttpClient _httpClient;

    public TransacaoService(ITransacaoRepository repository, IEventPublisher publisher, HttpClient httpClient)
    {
        _repository = repository;
        _publisher = publisher;
        _httpClient = httpClient;
    }

    public async Task<Result<TransacaoModel>> AutorizarAsync(TransacaoDto dto)
    {
        var transacaoResult = TransacaoModel.Criar(dto.IdCliente, dto.ValorSimulacao);
        if (!transacaoResult.IsSuccess)
            return transacaoResult;

        ClienteResponse? cliente;
        try
        {
            cliente = await _httpClient.GetFromJsonAsync<ClienteResponse>($"clientes/{dto.IdCliente}");
        }
        catch
        {
            return Result<TransacaoModel>.Failure("Cliente não encontrado.");
        }

        if (cliente == null)
            return Result<TransacaoModel>.Failure("Cliente não encontrado.");

        if (cliente.ValorLimite < dto.ValorSimulacao)
            return Result<TransacaoModel>.Failure("Limite insuficiente.");

        _repository.Criar(transacaoResult.Data!);
        await _publisher.PublicarAsync(new TransacaoAprovadaEvent(transacaoResult.Data!.ClienteId, transacaoResult.Data!.Valor));

        return transacaoResult;
    }
}
