using Moq;
using TechfinChallenge.Clientes.Api.DTOs;
using TechfinChallenge.Clientes.Api.Models;
using TechfinChallenge.Clientes.Api.Repositories;
using TechfinChallenge.Clientes.Api.Services;

namespace TechfinChallenge.Tests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _service = new ClienteService(_repositoryMock.Object);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarErro_QuandoLimiteNegativo()
    {
        var dto = new ClienteDto { Nome = "João", Cpf = "12345678901", ValorLimite = -100 };

        var result = _service.CadastrarCliente(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Valor de limite não pode ser negativo.", result.Error);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarErro_QuandoNomeVazio()
    {
        var dto = new ClienteDto { Nome = "", Cpf = "12345678901", ValorLimite = 500 };

        var result = _service.CadastrarCliente(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Nome é obrigatório.", result.Error);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarErro_QuandoCpfInvalido()
    {
        var dto = new ClienteDto { Nome = "João", Cpf = "123", ValorLimite = 500 };

        var result = _service.CadastrarCliente(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("CPF deve ter 11 dígitos.", result.Error);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarErro_QuandoCpfJaCadastrado()
    {
        var dto = new ClienteDto { Nome = "João", Cpf = "12345678901", ValorLimite = 500 };

        _repositoryMock
            .Setup(r => r.BuscarPorCpf("12345678901"))
            .Returns(new Cliente { Id = "1", Nome = "João", Cpf = "12345678901", ValorLimite = 500 });

        var result = _service.CadastrarCliente(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("CPF já cadastrado.", result.Error);
    }

    [Fact]
    public void CadastrarCliente_DeveRetornarCliente_QuandoDadosValidos()
    {
        var dto = new ClienteDto { Nome = "Maria", Cpf = "98765432100", ValorLimite = 1000 };

        _repositoryMock
            .Setup(r => r.BuscarPorCpf("98765432100"))
            .Returns((Cliente?)null);

        var result = _service.CadastrarCliente(dto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Id);
        Assert.Equal("Maria", result.Data.Nome);
        Assert.Equal("98765432100", result.Data.Cpf);
        Assert.Equal(1000, result.Data.ValorLimite);
    }
}
