using Dapper;
using Npgsql;
using TechfinChallenge.Transacao.Api.Data;
using Modelo = TechfinChallenge.Transacao.Api.Models.TransacaoModel;

namespace TechfinChallenge.Transacao.Api.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private NpgsqlConnection Conn() => new NpgsqlConnection(DatabaseInitializer.ConnectionString);

    public virtual void Criar(Modelo transacao)
    {
        using var db = Conn();
        db.Execute(
            "INSERT INTO Transacoes (Id, ClienteId, Valor, Status, Tipo) VALUES (@Id, @ClienteId, @Valor, @Status, @Tipo)",
            transacao);
    }

    public virtual Modelo? BuscarPorId(string id)
    {
        using var db = Conn();
        return db.QueryFirstOrDefault<Modelo>(
            "SELECT * FROM Transacoes WHERE Id = @Id", new { Id = id });
    }

    public virtual IEnumerable<Modelo> ListarTodos()
    {
        using var db = Conn();
        return db.Query<Modelo>("SELECT * FROM Transacoes").ToList();
    }
}
