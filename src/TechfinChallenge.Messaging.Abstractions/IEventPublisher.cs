namespace TechfinChallenge.Messaging.Abstractions;

public interface IEventPublisher
{
    Task PublicarAsync(TransacaoAprovadaEvent evento);
}
