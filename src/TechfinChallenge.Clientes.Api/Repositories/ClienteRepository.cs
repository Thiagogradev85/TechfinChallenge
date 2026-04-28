using Dapper;
using Npgsql;
using TechfinChallenge.Clientes.Api.Data;
using TechfinChallenge.Clientes.Api.Models;

namespace TechfinChallenge.Clientes.Api.Repositories;

public class ClienteRepository : IClienteRepository
{
    private NpgsqlConnection Conn() => new NpgsqlConnection(DatabaseInitializer.ConnectionString);

    public virtual Cliente? BuscarPorCpf(string cpf)
    {
        using var db = Conn();
        return db.QueryFirstOrDefault<Cliente>(
            "SELECT * FROM Clientes WHERE Cpf = @Cpf", new { Cpf = cpf });
    }

    public virtual Cliente? BuscarPorId(string id)
    {
        using var db = Conn();
        return db.QueryFirstOrDefault<Cliente>(
            "SELECT * FROM Clientes WHERE Id = @Id", new { Id = id });
    }

    public virtual void Criar(Cliente cliente)
    {
        using var db = Conn();
        db.Execute(
            "INSERT INTO Clientes (Id, Nome, Cpf, ValorLimite) VALUES (@Id, @Nome, @Cpf, @ValorLimite)",
            cliente);
    }

    public virtual IEnumerable<Cliente> ListarTodos()
    {
        using var db = Conn();
        return db.Query<Cliente>("SELECT * FROM Clientes").ToList();
    }

    public virtual void AtualizarLimite(string id, decimal novoLimite)
    {
        using var db = Conn();
        db.Execute(
            "UPDATE Clientes SET ValorLimite = @ValorLimite WHERE Id = @Id",
            new { ValorLimite = novoLimite, Id = id });
    }

    public virtual void Atualizar(string id, Cliente cliente)
    {
        using var db = Conn();
        db.Execute(
            "UPDATE Clientes SET Nome = @Nome, Cpf = @Cpf, ValorLimite = @ValorLimite WHERE Id = @Id",
            new { cliente.Nome, cliente.Cpf, cliente.ValorLimite, Id = id });
    }

    public virtual void Deletar(string id)
    {
        using var db = Conn();
        db.Execute("DELETE FROM Clientes WHERE Id = @Id", new { Id = id });
    }
}
