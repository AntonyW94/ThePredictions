namespace ThePredictions.Application.Data;

/// <summary>
/// Executes a database operation with automatic retry on transient SQL failures.
/// </summary>
public interface ISqlRetryPolicy
{
    /// <summary>
    /// Executes the supplied asynchronous operation, retrying on transient SQL errors
    /// with exponential back-off.
    /// </summary>
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);
}
