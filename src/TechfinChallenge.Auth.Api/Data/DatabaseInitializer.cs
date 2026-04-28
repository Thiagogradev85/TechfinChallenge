using Dapper;
using Npgsql;

namespace TechfinChallenge.Auth.Api.Data;

public class DatabaseInitializer
{
    public static string ConnectionString { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        ConnectionString = connectionString;

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Usuarios (
                Id TEXT PRIMARY KEY,
                Email TEXT NOT NULL UNIQUE,
                SenhaHash TEXT NOT NULL
            )");
    }
}
