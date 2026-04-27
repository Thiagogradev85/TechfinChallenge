using Dapper;
using Npgsql;

namespace TechfinChallenge.Clientes.Api.Data;

public class DatabaseInitializer
{
    public static NpgsqlConnection Connection { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();

        Connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Clientes (
                Id TEXT PRIMARY KEY,
                Nome TEXT NOT NULL,
                Cpf TEXT NOT NULL UNIQUE,
                ValorLimite NUMERIC(18,2) NOT NULL
            )");
    }
}
