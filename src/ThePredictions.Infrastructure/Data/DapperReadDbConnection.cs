using Dapper;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;

namespace ThePredictions.Infrastructure.Data;

public class DapperReadDbConnection(IDbConnectionFactory connectionFactory, ISqlRetryPolicy retryPolicy, IOptions<TimeoutSettings> timeoutSettings) : IApplicationReadDbConnection
{
    private readonly int _commandTimeout = timeoutSettings.Value.DatabaseCommandTimeoutSeconds;

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        return await retryPolicy.ExecuteAsync(async ct =>
        {
            var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: ct, commandTimeout: _commandTimeout);

            using var connection = connectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(command);
        }, cancellationToken);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        return await retryPolicy.ExecuteAsync(async ct =>
        {
            var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: ct, commandTimeout: _commandTimeout);

            using var connection = connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(command);
        }, cancellationToken);
    }
}
