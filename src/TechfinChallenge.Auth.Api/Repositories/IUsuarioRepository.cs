using TechfinChallenge.Auth.Api.Models;

namespace TechfinChallenge.Auth.Api.Repositories;

public interface IUsuarioRepository
{
    Usuario? BuscarPorEmail(string email);
    void Criar(Usuario usuario);
}
