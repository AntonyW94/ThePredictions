using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Common.Exceptions;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;

namespace ThePredictions.Persistence.SqlServer.Data;

public class DapperReadDbConnection(
    IDbConnectionFactory connectionFactory,
    ISqlRetryPolicy retryPolicy,
    IReadIsolationPolicy isolationPolicy,
    IOptions<TimeoutSettings> timeoutSettings,
    IOptions<QueryMonitoringSettings> queryMonitoringSettings,
    ILogger<DapperReadDbConnection> logger) : IApplicationReadDbConnection
{
    private readonly int _commandTimeout = timeoutSettings.Value.DatabaseCommandTimeoutSeconds;
    private readonly int _slowQueryThresholdMilliseconds = queryMonitoringSettings.Value.SlowQueryThresholdMilliseconds;
    private readonly int _slowConnectionThresholdMilliseconds = queryMonitoringSettings.Value.SlowConnectionThresholdMilliseconds;

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        return await ExecuteTimedAsync(sql, param, (connection, command) => connection.QueryAsync<T>(command), cancellationToken);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken, object? param = null)
    {
        return await ExecuteTimedAsync(sql, param, (connection, command) => connection.QuerySingleOrDefaultAsync<T>(command), cancellationToken);
    }

    // Runs the (retried) read and logs a Warning when it takes at least the configured threshold, so
    // slow query paths are visible in the logs. Parameters are never logged - the SQL is parameterised,
    // so the text carries no user data. Timing wraps the whole retried execution (retry overhead counts
    // as slow too); the finally block ensures a slow failure is still reported.
    //
    // The elapsed time is broken down rather than reported as a single number, because "this read took
    // two seconds" has three quite different causes and the fix for each is unrelated:
    //
    //   - a high ConnectionMilliseconds is time spent getting a connection at all, not running SQL:
    //     an exhausted pool, or the cost of a fresh login handshake to a remote server.
    //   - a high remainder (Elapsed minus Connection) is the server: the query itself, or waiting on
    //     locks held by a writer.
    //   - both low while Elapsed is high means the time went nowhere near the database. The await
    //     resumed late because no thread was free, which QueuedWorkItems is here to show.
    //
    // Without the split every one of those looks identical in the log, and the natural reading - that
    // the query needs an index - is the wrong conclusion for two of the three.
    //
    // Reporting the breakdown was not enough on its own: the *decision* to warn still compared the
    // total against one threshold, so the first two causes both arrived as "Slow query" and the
    // reader had to re-derive which one it was every time. The two now have a threshold and a message
    // each, so the log line names its own cause. Both can fire for a single read - that is a read
    // with both problems, and each line stands on its own.
    private async Task<TResult> ExecuteTimedAsync<TResult>(
        string sql,
        object? param,
        Func<IDbConnection, CommandDefinition, Task<TResult>> read,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // Assigned inside the retried operation, so on a retried read this reports the last attempt's
        // connection cost rather than the sum. The total covers every attempt either way.
        var connectionMilliseconds = 0L;

        try
        {
            // Every read runs at the isolation level the policy chooses, applied here rather than in each
            // query so there is one answer to "what level do reads run at" instead of one per file. The
            // logged SQL stays the caller's own text: the wrapper is noise in a slow-query warning, and it
            // is the same on every line.
            var commandText = isolationPolicy.Apply(sql);

            return await retryPolicy.ExecuteAsync(async ct =>
            {
                var command = new CommandDefinition(commandText: commandText, parameters: param, cancellationToken: ct, commandTimeout: _commandTimeout);

                var connectionStopwatch = Stopwatch.StartNew();

                using var connection = connectionFactory.CreateConnection();

                // Dapper would open this itself on first use, which would fold the wait for a pooled
                // connection into the query's measured cost. Opening it here keeps the two apart. A
                // factory that hands back an open connection has nothing to do; Dapper leaves a
                // connection it did not open alone, and the using block still disposes it.
                if (connection.State != ConnectionState.Open)
                    await ((DbConnection)connection).OpenAsync(ct);

                connectionStopwatch.Stop();
                connectionMilliseconds = connectionStopwatch.ElapsedMilliseconds;

                return await read(connection, command);
            }, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            // Dapper reports a result-set/result-record mismatch ("A parameterless default constructor or
            // one matching signature ... is required for ... materialization") and a single-row query that
            // returned multiple rows as a plain InvalidOperationException. Wrapping it names the failure in
            // the log message instead of leaving a bare Dapper exception to be interpreted.
            //
            // This no longer decides the severity: InvalidOperationException is itself reported as an Error
            // and a 500 (see ADR-0016), so an untranslated read fault is filed correctly regardless.
            throw new ReadQueryFailedException(exception);
        }
        finally
        {
            stopwatch.Stop();

            // Never negative: the connection stopwatch runs strictly inside the outer one. On a retried
            // read the outer covers every attempt while connectionMilliseconds is only the last one's,
            // so the remainder absorbs the abandoned attempts - which belongs with the query half, as
            // it is time the read spent waiting on the server rather than on the pool.
            var queryMilliseconds = stopwatch.ElapsedMilliseconds - connectionMilliseconds;

            if (connectionMilliseconds >= _slowConnectionThresholdMilliseconds)
                logger.LogWarning(
                    "Slow connection acquisition ({ConnectionMilliseconds}ms >= {ConnectionThresholdMilliseconds}ms threshold, {ElapsedMilliseconds}ms for the whole read, {QueuedWorkItems} work items queued on {WorkerThreads} threads): {Sql}",
                    connectionMilliseconds,
                    _slowConnectionThresholdMilliseconds,
                    stopwatch.ElapsedMilliseconds,
                    ThreadPool.PendingWorkItemCount,
                    ThreadPool.ThreadCount,
                    sql);

            if (queryMilliseconds >= _slowQueryThresholdMilliseconds)
                logger.LogWarning(
                    "Slow query ({QueryMilliseconds}ms >= {ThresholdMilliseconds}ms threshold, {ElapsedMilliseconds}ms for the whole read including {ConnectionMilliseconds}ms acquiring the connection, {QueuedWorkItems} work items queued on {WorkerThreads} threads): {Sql}",
                    queryMilliseconds,
                    _slowQueryThresholdMilliseconds,
                    stopwatch.ElapsedMilliseconds,
                    connectionMilliseconds,
                    ThreadPool.PendingWorkItemCount,
                    ThreadPool.ThreadCount,
                    sql);
        }
    }
}
