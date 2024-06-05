using System.Data;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using Npgsql;

namespace AthenasAcademy.Certificate.Core;

public class BaseRepository
{
    private readonly PostgreSettings _settings;
    public BaseRepository(PostgreSettings settings)
    {
        _settings = settings;
    }

    public async Task<IDbConnection> GetConnectionAsync()
    {
        NpgsqlConnection connection = new(GetConnectionString());
        await connection.OpenAsync();
        return connection;
    }

    public IDbConnection GetConnection()
    {
        NpgsqlConnection connection = new(GetConnectionString());
        connection.Open();
        return connection;
    }

    private string GetConnectionString()
    {
        NpgsqlConnectionStringBuilder connectionString = new()
        {
            Host = _settings.Host,
            Port = _settings.Port,
            Database = _settings.Database,
            Username = _settings.Username,
            Password = _settings.Password,
            Timeout = 20,
            CommandTimeout = 20,
            SslMode = _settings.SslMode ? 
                        SslMode.Disable : SslMode.Allow,
        };
        return connectionString.ConnectionString;
    }
}
