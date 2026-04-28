using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TechfinChallenge.Clientes.Api.Messaging;
using TechfinChallenge.Clientes.Api.Models;
using TechfinChallenge.Clientes.Api.Services;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Tests;

public class TransacaoEventHandlerTests
{
    private TransacaoEventHandler CriarHandler(Mock<IClienteService> serviceMock)
    {
        return new TransacaoEventHandler(serviceMock.Object, NullLogger<TransacaoEventHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_DeveDebitarLimite_QuandoTipoDebito()
    {
        var cliente = new Cliente { Id = "1", Nome = "João", ValorLimite = 1000 };
        var serviceMock = new Mock<IClienteService>();
        serviceMock.Setup(s => s.BuscarPorId("1")).Returns(cliente);

        var handler = CriarHandler(serviceMock);
        var evento = new TransacaoAprovadaEvent("1", 300, "debito");

        await handler.HandleAsync(evento);

        // verifica que AtualizarLimite foi chamado com 1000 - 300 = 700
        serviceMock.Verify(s => s.AtualizarLimite("1", 700), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveCreditarLimite_QuandoTipoCredito()
    {
        var cliente = new Cliente { Id = "1", Nome = "João", ValorLimite = 1000 };
        var serviceMock = new Mock<IClienteService>();
        serviceMock.Setup(s => s.BuscarPorId("1")).Returns(cliente);

        var handler = CriarHandler(serviceMock);
        var evento = new TransacaoAprovadaEvent("1", 500, "credito");

        await handler.HandleAsync(evento);

        // verifica que AtualizarLimite foi chamado com 1000 + 500 = 1500
        serviceMock.Verify(s => s.AtualizarLimite("1", 1500), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveIgnorar_QuandoClienteNaoEncontrado()
    {
        var serviceMock = new Mock<IClienteService>();
        serviceMock.Setup(s => s.BuscarPorId("inexistente")).Returns((Cliente?)null);

        var handler = CriarHandler(serviceMock);
        var evento = new TransacaoAprovadaEvent("inexistente", 100, "debito");

        await handler.HandleAsync(evento);

        // AtualizarLimite NÃO deve ser chamado — cliente não existe
        serviceMock.Verify(s => s.AtualizarLimite(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DeveLancarExcecao_QuandoServicoFalha()
    {
        var serviceMock = new Mock<IClienteService>();
        serviceMock.Setup(s => s.BuscarPorId("1")).Returns(new Cliente { Id = "1", Nome = "João", ValorLimite = 1000 });
        serviceMock.Setup(s => s.AtualizarLimite(It.IsAny<string>(), It.IsAny<decimal>()))
                   .Throws(new Exception("Banco fora do ar!"));

        var handler = CriarHandler(serviceMock);
        var evento = new TransacaoAprovadaEvent("1", 100, "debito");

        // Este é o cenário que dispara o retry no KafkaConsumer:
        // se o handler lança exceção, o offset não é commitado
        // e o Kafka reentrega a mensagem (at-least-once)
        await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(evento));
    }
}
