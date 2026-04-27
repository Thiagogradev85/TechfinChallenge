namespace TechfinChallenge.Messaging.Abstractions;

public interface ITransacaoEventHandler
{
    Task HandleAsync(TransacaoAprovadaEvent evento);
}
