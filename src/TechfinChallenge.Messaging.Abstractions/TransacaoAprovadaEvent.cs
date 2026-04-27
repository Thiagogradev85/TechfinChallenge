namespace TechfinChallenge.Messaging.Abstractions;

public record TransacaoAprovadaEvent(string ClienteId, decimal Valor);
