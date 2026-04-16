using System.Data;

namespace ThePredictions.Application.Data;

public interface IDbTransactionContext
{
    bool HasActiveTransaction { get; }
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    Task BeginAsync(CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);
}
