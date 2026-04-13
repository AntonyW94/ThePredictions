using Dapper;
using ThePredictions.Application.Data;

namespace ThePredictions.Infrastructure.Data;

public class DapperReadDbConnection(IDbConnectionFactory connectionFactory, ISqlRetryPolicy retryPolicy) : IApplicationReadDbConnection
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        return await retryPolicy.ExecuteAsync(async ct =>
        {
            var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: ct);

            using var connection = connectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(command);
        }, cancellationToken);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        return await retryPolicy.ExecuteAsync(async ct =>
        {
            var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: ct);

            using var connection = connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(command);
        }, cancellationToken);
    }
}
