using TechfinChallenge.Transacao.Api.Models;

namespace TechfinChallenge.Transacao.Api.Repositories;

public interface ITransacaoRepository
{
    void Criar(TransacaoModel transacao);
    TransacaoModel? BuscarPorId(string id);
}
