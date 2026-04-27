using Dapper;
using TechfinChallenge.Auth.Api.Data;
using TechfinChallenge.Auth.Api.Models;

namespace TechfinChallenge.Auth.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    public Usuario? BuscarPorEmail(string email)
    {
        return DatabaseInitializer.Connection.QueryFirstOrDefault<Usuario>(
            "SELECT * FROM Usuarios WHERE Email = @Email",
            new { Email = email });
    }

    public void Criar(Usuario usuario)
    {
        DatabaseInitializer.Connection.Execute(
            "INSERT INTO Usuarios (Id, Email, SenhaHash) VALUES (@Id, @Email, @SenhaHash)",
            usuario);
    }
}
