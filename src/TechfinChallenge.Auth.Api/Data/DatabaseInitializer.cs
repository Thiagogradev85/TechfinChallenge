using Dapper;
using Npgsql;

namespace TechfinChallenge.Auth.Api.Data;

public class DatabaseInitializer
{
    public static NpgsqlConnection Connection { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();

        Connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Usuarios (
                Id TEXT PRIMARY KEY,
                Email TEXT NOT NULL UNIQUE,
                SenhaHash TEXT NOT NULL
            )");
    }
}
