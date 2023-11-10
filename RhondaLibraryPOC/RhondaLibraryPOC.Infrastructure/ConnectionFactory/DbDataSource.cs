
using Npgsql;
using RhondaLibraryPOC.Application.Interfaces.Persistence;
using System.Data;

namespace RhondaLibraryPOC.Infrastructure.ConnectionFactory;

public class DbDataSource : IDataSource
{
    private readonly string? _connectionString;

    public DbDataSource(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
