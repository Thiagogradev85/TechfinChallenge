using System.Net.Http.Json;
using TechfinChallenge.Transacao.Api.DTOs;
using TechfinChallenge.Transacao.Api.Messaging;
using Modelo = TechfinChallenge.Transacao.Api.Models.TransacaoModel;
using TechfinChallenge.Transacao.Api.Repositories;

namespace TechfinChallenge.Transacao.Api.Services;

public class TransacaoService
{
    private readonly TransacaoRepository _repository;
    private readonly IRabbitMqPublisher _publisher;
    private readonly HttpClient _httpClient;

    public TransacaoService(TransacaoRepository repository, IRabbitMqPublisher publisher, HttpClient httpClient)
    {
        _repository = repository;
        _publisher = publisher;
        _httpClient = httpClient;
    }

    public async Task<(string? id, string? erro)> AutorizarAsync(TransacaoDto dto)
    {
        if (dto.ValorSimulacao <= 0)
            return (null, "Valor deve ser maior que zero.");

        ClienteResponse? cliente;
        try
        {
            cliente = await _httpClient.GetFromJsonAsync<ClienteResponse>($"clientes/{dto.IdCliente}");
        }
        catch
        {
            return (null, "Cliente não encontrado.");
        }

        if (cliente == null)
            return (null, "Cliente não encontrado.");

        if (cliente.ValorLimite < dto.ValorSimulacao)
            return (null, "Limite insuficiente.");

        var transacao = new Modelo
        {
            Id = Guid.NewGuid().ToString(),
            ClienteId = dto.IdCliente,
            Valor = dto.ValorSimulacao,
            Status = "aprovado"
        };

        _repository.Criar(transacao);
        _publisher.Publicar(transacao);

        return (transacao.Id, null);
    }
}
