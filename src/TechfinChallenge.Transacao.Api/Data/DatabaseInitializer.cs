using Dapper;
using Npgsql;

namespace TechfinChallenge.Transacao.Api.Data;

public class DatabaseInitializer
{
    public static NpgsqlConnection Connection { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();

        Connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Transacoes (
                Id TEXT PRIMARY KEY,
                ClienteId TEXT NOT NULL,
                Valor NUMERIC(18,2) NOT NULL,
                Status TEXT NOT NULL
            )");
    }
}
