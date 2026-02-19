using Dapper;
using ThePredictions.Application.Data;

namespace ThePredictions.Infrastructure.Data;

public class DapperReadDbConnection(IDbConnectionFactory connectionFactory) : IApplicationReadDbConnection
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: cancellationToken);

        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(command);
    }
   
    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: cancellationToken);

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(command);
    }
}