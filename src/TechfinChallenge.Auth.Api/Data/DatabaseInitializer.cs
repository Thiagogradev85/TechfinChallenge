using Dapper;
using Microsoft.Data.Sqlite;

namespace TechfinChallenge.Auth.Api.Data;

public class DatabaseInitializer
{
    public static SqliteConnection Connection { get; private set; } = null!;

    public static void Initialize(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
        Connection.Open();

        Connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Usuarios (
                Id TEXT PRIMARY KEY,
                Email TEXT NOT NULL UNIQUE,
                SenhaHash TEXT NOT NULL
            )");
    }
}
