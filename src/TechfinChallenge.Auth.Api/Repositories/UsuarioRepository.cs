using Dapper;
using Npgsql;
using TechfinChallenge.Auth.Api.Data;
using TechfinChallenge.Auth.Api.Models;

namespace TechfinChallenge.Auth.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private NpgsqlConnection Conn() => new NpgsqlConnection(DatabaseInitializer.ConnectionString);

    public Usuario? BuscarPorEmail(string email)
    {
        using var db = Conn();
        return db.QueryFirstOrDefault<Usuario>(
            "SELECT * FROM Usuarios WHERE Email = @Email", new { Email = email });
    }

    public void Criar(Usuario usuario)
    {
        using var db = Conn();
        db.Execute(
            "INSERT INTO Usuarios (Id, Email, SenhaHash) VALUES (@Id, @Email, @SenhaHash)",
            usuario);
    }
}
