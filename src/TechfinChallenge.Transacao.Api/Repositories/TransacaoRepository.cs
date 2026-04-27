using Dapper;
using TechfinChallenge.Transacao.Api.Data;
using Modelo = TechfinChallenge.Transacao.Api.Models.TransacaoModel;

namespace TechfinChallenge.Transacao.Api.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    public virtual void Criar(Modelo transacao)
    {
        DatabaseInitializer.Connection.Execute(
            "INSERT INTO Transacoes (Id, ClienteId, Valor, Status) VALUES (@Id, @ClienteId, @Valor, @Status)",
            transacao);
    }

    public virtual Modelo? BuscarPorId(string id)
    {
        return DatabaseInitializer.Connection.QueryFirstOrDefault<Modelo>(
            "SELECT * FROM Transacoes WHERE Id = @Id",
            new { Id = id });
    }

    public virtual IEnumerable<Modelo> ListarTodos()
    {
        return DatabaseInitializer.Connection.Query<Modelo>(
            "SELECT * FROM Transacoes");
    }
}
