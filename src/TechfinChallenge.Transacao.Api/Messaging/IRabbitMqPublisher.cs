using Modelo = TechfinChallenge.Transacao.Api.Models.TransacaoModel;

namespace TechfinChallenge.Transacao.Api.Messaging;

public interface IRabbitMqPublisher
{
    void Publicar(Modelo transacao);
}
