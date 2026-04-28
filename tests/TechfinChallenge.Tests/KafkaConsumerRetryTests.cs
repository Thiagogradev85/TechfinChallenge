using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text.Json;
using TechfinChallenge.Messaging.Abstractions;
using TechfinChallenge.Messaging.Kafka;

namespace TechfinChallenge.Tests;

public class KafkaConsumerRetryTests
{
    // Monta um KafkaConsumer com um IServiceProvider que retorna o handler mockado
    private static (KafkaConsumer consumer, Mock<IConsumer<string, string>> consumerMock, Mock<IProducer<string, string>> dlqMock)
        CriarSetup(ITransacaoEventHandler handler)
    {
        var services = new ServiceCollection();
        services.AddScoped<ITransacaoEventHandler>(_ => handler);
        var sp = services.BuildServiceProvider();

        var consumer = new KafkaConsumer(sp, NullLogger<KafkaConsumer>.Instance);

        var consumerMock = new Mock<IConsumer<string, string>>();
        // Commit(ConsumeResult<>) é método void da interface — sem .Returns()
        consumerMock.Setup(c => c.Commit(It.IsAny<ConsumeResult<string, string>>()));

        var dlqMock = new Mock<IProducer<string, string>>();
        dlqMock
            .Setup(p => p.Flush(It.IsAny<TimeSpan>()))
            .Returns(0);

        return (consumer, consumerMock, dlqMock);
    }

    private static ConsumeResult<string, string> CriarConsumeResult(TransacaoAprovadaEvent evento) =>
        new ConsumeResult<string, string>
        {
            TopicPartitionOffset = new TopicPartitionOffset("transacao.aprovada", new Partition(0), new Offset(0)),
            Message = new Message<string, string>
            {
                Key = evento.ClienteId,
                Value = JsonSerializer.Serialize(evento)
            }
        };

    [Fact]
    public void ProcessWithRetry_DeveChamarHandlerMaxRetries_QuandoHandlerSempreLanca()
    {
        // Arrange — handler que SEMPRE lança exceção (simula banco fora do ar)
        var handlerMock = new Mock<ITransacaoEventHandler>();
        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TransacaoAprovadaEvent>()))
            .ThrowsAsync(new Exception("Banco fora do ar!"));

        var (consumer, consumerMock, dlqMock) = CriarSetup(handlerMock.Object);
        var evento = new TransacaoAprovadaEvent("1", 100, "debito");
        var result = CriarConsumeResult(evento);

        // Act — TimeSpan.Zero evita o Thread.Sleep de 1s+2s nos testes
        consumer.ProcessWithRetry(evento, result, consumerMock.Object, dlqMock.Object, TimeSpan.Zero);

        // Assert — handler chamado exatamente MaxRetries (3) vezes
        handlerMock.Verify(
            h => h.HandleAsync(It.IsAny<TransacaoAprovadaEvent>()),
            Times.Exactly(3));

        // Assert — DLQ deve receber a mensagem com o topic "transacao.falha"
        dlqMock.Verify(
            p => p.Produce(
                "transacao.falha",
                It.IsAny<Message<string, string>>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()),
            Times.Once);

        // Assert — offset commitado mesmo após falha (partição não pode ficar bloqueada)
        consumerMock.Verify(
            c => c.Commit(It.IsAny<ConsumeResult<string, string>>()),
            Times.Once);
    }

    [Fact]
    public void ProcessWithRetry_NaoDeveChamarDlq_QuandoSegundaTentativaSucede()
    {
        // Arrange — handler que falha na 1ª tentativa mas sucede na 2ª (falha temporária)
        var handlerMock = new Mock<ITransacaoEventHandler>();
        handlerMock
            .SetupSequence(h => h.HandleAsync(It.IsAny<TransacaoAprovadaEvent>()))
            .ThrowsAsync(new Exception("Timeout temporário!"))
            .Returns(Task.CompletedTask);

        var (consumer, consumerMock, dlqMock) = CriarSetup(handlerMock.Object);
        var evento = new TransacaoAprovadaEvent("1", 200, "credito");
        var result = CriarConsumeResult(evento);

        // Act
        consumer.ProcessWithRetry(evento, result, consumerMock.Object, dlqMock.Object, TimeSpan.Zero);

        // Assert — handler chamado 2 vezes (1ª falha + 2ª sucesso)
        handlerMock.Verify(
            h => h.HandleAsync(It.IsAny<TransacaoAprovadaEvent>()),
            Times.Exactly(2));

        // Assert — DLQ NÃO chamado: o retry resolveu antes de esgotar
        dlqMock.Verify(
            p => p.Produce(
                It.IsAny<string>(),
                It.IsAny<Message<string, string>>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()),
            Times.Never);

        // Assert — offset commitado normalmente
        consumerMock.Verify(
            c => c.Commit(It.IsAny<ConsumeResult<string, string>>()),
            Times.Once);
    }

    [Fact]
    public void ProcessWithRetry_NaoDeveChamarDlq_QuandoPrimeiraTentativaSucede()
    {
        // Arrange — handler que funciona na primeira tentativa (caminho feliz)
        var handlerMock = new Mock<ITransacaoEventHandler>();
        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TransacaoAprovadaEvent>()))
            .Returns(Task.CompletedTask);

        var (consumer, consumerMock, dlqMock) = CriarSetup(handlerMock.Object);
        var evento = new TransacaoAprovadaEvent("1", 50, "debito");
        var result = CriarConsumeResult(evento);

        // Act
        consumer.ProcessWithRetry(evento, result, consumerMock.Object, dlqMock.Object, TimeSpan.Zero);

        // Assert — handler chamado apenas 1 vez
        handlerMock.Verify(
            h => h.HandleAsync(It.IsAny<TransacaoAprovadaEvent>()),
            Times.Once);

        // Assert — DLQ nunca chamado
        dlqMock.Verify(
            p => p.Produce(
                It.IsAny<string>(),
                It.IsAny<Message<string, string>>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()),
            Times.Never);

        // Assert — offset commitado
        consumerMock.Verify(
            c => c.Commit(It.IsAny<ConsumeResult<string, string>>()),
            Times.Once);
    }
}
