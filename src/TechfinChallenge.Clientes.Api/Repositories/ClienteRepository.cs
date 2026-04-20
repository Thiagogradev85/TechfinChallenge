using Dapper;
using TechfinChallenge.Clientes.Api.Data;
using TechfinChallenge.Clientes.Api.Models;

namespace TechfinChallenge.Clientes.Api.Repositories;

public class ClienteRepository
{
    public Cliente? BuscarPorCpf(string cpf)
    {
        return DatabaseInitializer.Connection.QueryFirstOrDefault<Cliente>(
            "SELECT * FROM Clientes WHERE Cpf = @Cpf",
            new { Cpf = cpf });
    }

    public Cliente? BuscarPorId(string id)
    {
        return DatabaseInitializer.Connection.QueryFirstOrDefault<Cliente>(
            "SELECT * FROM Clientes WHERE Id = @Id",
            new { Id = id });
    }

    public void Criar(Cliente cliente)
    {
        DatabaseInitializer.Connection.Execute(
            "INSERT INTO Clientes (Id, Nome, Cpf, ValorLimite) VALUES (@Id, @Nome, @Cpf, @ValorLimite)",
            cliente);
    }

    public IEnumerable<Cliente> ListarTodos()
    {
        return DatabaseInitializer.Connection.Query<Cliente>(
            "SELECT * FROM Clientes");
    }

    public void AtualizarLimite(string id, decimal novoLimite)
    {
        DatabaseInitializer.Connection.Execute(
            "UPDATE Clientes SET ValorLimite = @ValorLimite WHERE Id = @Id",
            new { ValorLimite = novoLimite, Id = id });
    }
}
