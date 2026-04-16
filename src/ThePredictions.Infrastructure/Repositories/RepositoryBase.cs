using ThePredictions.Application.Data;
using System.Data;

namespace ThePredictions.Infrastructure.Repositories;

public abstract class RepositoryBase(IDbConnectionFactory connectionFactory, IDbTransactionContext transactionContext)
{
    protected IDbConnection Connection => transactionContext.HasActiveTransaction
        ? transactionContext.Connection
        : connectionFactory.CreateConnection();

    protected IDbTransaction? Transaction => transactionContext.HasActiveTransaction
        ? transactionContext.Transaction
        : null;
}
