using System.Net;
using System.Text.Json;
using Moq;
using TechfinChallenge.Transacao.Api.DTOs;
using TechfinChallenge.Transacao.Api.Messaging;
using TechfinChallenge.Transacao.Api.Repositories;
using TechfinChallenge.Transacao.Api.Services;

namespace TechfinChallenge.Tests;

public class TransacaoServiceTests
{
    private TransacaoService CriarService(ClienteResponse? clienteResponse)
    {
        var repositoryMock = new Mock<TransacaoRepository>();
        var publisherMock = new Mock<IRabbitMqPublisher>();

        var handler = new FakeHttpHandler(clienteResponse);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };

        return new TransacaoService(repositoryMock.Object, publisherMock.Object, httpClient);
    }

    [Fact]
    public async Task AutorizarAsync_DeveRetornarErro_QuandoValorZero()
    {
        var service = CriarService(null);
        var dto = new TransacaoDto { IdCliente = "1", ValorSimulacao = 0 };

        var (id, erro) = await service.AutorizarAsync(dto);

        Assert.Null(id);
        Assert.Equal("Valor deve ser maior que zero.", erro);
    }

    [Fact]
    public async Task AutorizarAsync_DeveRetornarErro_QuandoClienteNaoEncontrado()
    {
        var service = CriarService(null);
        var dto = new TransacaoDto { IdCliente = "inexistente", ValorSimulacao = 100 };

        var (id, erro) = await service.AutorizarAsync(dto);

        Assert.Null(id);
        Assert.Equal("Cliente não encontrado.", erro);
    }

    [Fact]
    public async Task AutorizarAsync_DeveRetornarErro_QuandoLimiteInsuficiente()
    {
        var cliente = new ClienteResponse("1", "João", "12345678901", 50);
        var service = CriarService(cliente);
        var dto = new TransacaoDto { IdCliente = "1", ValorSimulacao = 200 };

        var (id, erro) = await service.AutorizarAsync(dto);

        Assert.Null(id);
        Assert.Equal("Limite insuficiente.", erro);
    }

    [Fact]
    public async Task AutorizarAsync_DeveRetornarId_QuandoTransacaoAprovada()
    {
        var cliente = new ClienteResponse("1", "João", "12345678901", 1000);
        var service = CriarService(cliente);
        var dto = new TransacaoDto { IdCliente = "1", ValorSimulacao = 200 };

        var (id, erro) = await service.AutorizarAsync(dto);

        Assert.NotNull(id);
        Assert.Null(erro);
    }
}

public class FakeHttpHandler : HttpMessageHandler
{
    private readonly ClienteResponse? _response;

    public FakeHttpHandler(ClienteResponse? response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_response == null)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        var json = JsonSerializer.Serialize(_response);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
