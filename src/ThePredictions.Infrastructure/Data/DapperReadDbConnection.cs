using Dapper;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;

namespace ThePredictions.Infrastructure.Data;

public class DapperReadDbConnection(IDbConnectionFactory connectionFactory, IOptions<TimeoutSettings> timeoutSettings) : IApplicationReadDbConnection
{
    private readonly int _commandTimeout = timeoutSettings.Value.DatabaseCommandTimeoutSeconds;

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: cancellationToken, commandTimeout: _commandTimeout);

        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(command);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        var command = new CommandDefinition(commandText: sql, parameters: param, cancellationToken: cancellationToken, commandTimeout: _commandTimeout);

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(command);
    }
}