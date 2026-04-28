using Dapper;
using Npgsql;

namespace TechfinChallenge.Transacao.Api.Data;

public class DatabaseInitializer
{
    public static string ConnectionString { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        ConnectionString = connectionString;

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Transacoes (
                Id TEXT PRIMARY KEY,
                ClienteId TEXT NOT NULL,
                Valor NUMERIC(18,2) NOT NULL,
                Status TEXT NOT NULL,
                Tipo TEXT NOT NULL DEFAULT 'debito'
            )");

        connection.Execute(@"
            ALTER TABLE Transacoes ADD COLUMN IF NOT EXISTS Tipo TEXT NOT NULL DEFAULT 'debito'");
    }
}
