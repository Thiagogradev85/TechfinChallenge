using Dapper;
using Microsoft.Data.Sqlite;

namespace TechfinChallenge.Clientes.Api.Data;

public class DatabaseInitializer
{
    public static SqliteConnection Connection { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
        Connection.Open();

        Connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Clientes (
                Id TEXT PRIMARY KEY,
                Nome TEXT NOT NULL,
                Cpf TEXT NOT NULL UNIQUE,
                ValorLimite REAL NOT NULL
            )");
    }
}
