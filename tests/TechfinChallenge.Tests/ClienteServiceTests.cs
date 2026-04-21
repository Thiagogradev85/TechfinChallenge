using Moq;
using TechfinChallenge.Clientes.Api.DTOs;
using TechfinChallenge.Clientes.Api.Models;
using TechfinChallenge.Clientes.Api.Repositories;
using TechfinChallenge.Clientes.Api.Services;

namespace TechfinChallenge.Tests;

public class ClienteServiceTests
{
    private readonly Mock<ClienteRepository> _repositoryMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<ClienteRepository>();
        _service = new ClienteService(_repositoryMock.Object);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarErro_QuandoLimiteNegativo()
    {
        var dto = new ClienteDto { Nome = "João", Cpf = "12345678901", ValorLimite = -100 };

        var (id, erro) = _service.CadastrarCliente(dto);

        Assert.Null(id);
        Assert.Equal("Valor de limite não pode ser negativo.", erro);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarErro_QuandoCpfJaCadastrado()
    {
        var dto = new ClienteDto { Nome = "João", Cpf = "12345678901", ValorLimite = 500 };

        _repositoryMock
            .Setup(r => r.BuscarPorCpf("12345678901"))
            .Returns(new Cliente { Id = "1", Nome = "João", Cpf = "12345678901", ValorLimite = 500 });

        var (id, erro) = _service.CadastrarCliente(dto);

        Assert.Null(id);
        Assert.Equal("CPF já cadastrado.", erro);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarId_QuandoDadosValidos()
    {
        var dto = new ClienteDto { Nome = "Maria", Cpf = "98765432100", ValorLimite = 1000 };

        _repositoryMock
            .Setup(r => r.BuscarPorCpf("98765432100"))
            .Returns((Cliente?)null);

        var (id, erro) = _service.CadastrarCliente(dto);

        Assert.NotNull(id);
        Assert.Null(erro);
    }
}
