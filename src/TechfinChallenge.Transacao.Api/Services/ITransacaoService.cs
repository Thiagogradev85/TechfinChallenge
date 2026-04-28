using TechfinChallenge.Transacao.Api.DTOs;
using TechfinChallenge.Transacao.Api.Models;
using TechfinChallenge.Shared;

namespace TechfinChallenge.Transacao.Api.Services;

public interface ITransacaoService
{
    Task<Result<TransacaoModel>> AutorizarAsync(TransacaoDto dto);
    Task<IEnumerable<TransacaoModel>> ListarAsync();
}
