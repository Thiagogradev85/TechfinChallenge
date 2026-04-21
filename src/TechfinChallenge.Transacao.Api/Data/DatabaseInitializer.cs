using Dapper;
using Microsoft.Data.Sqlite;

namespace TechfinChallenge.Transacao.Api.Data;

public class DatabaseInitializer
{
    public static SqliteConnection Connection { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
        Connection.Open();
        Connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Transacoes (
                Id TEXT PRIMARY KEY,
                ClienteId TEXT NOT NULL,
                Valor REAL NOT NULL,
                Status TEXT NOT NULL
            )");
    }
}
