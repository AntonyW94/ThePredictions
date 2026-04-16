using MediatR;
using Microsoft.Extensions.Logging;
using ThePredictions.Application.Common.Interfaces;
using ThePredictions.Application.Data;

namespace ThePredictions.Application.Common.Behaviours;

public class TransactionBehaviour<TRequest, TResponse>(
    IDbTransactionContext transactionContext,
    ILogger<TransactionBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        await transactionContext.BeginAsync(cancellationToken);

        try
        {
            logger.LogDebug("Beginning transaction for {RequestName}", requestName);

            var response = await next(cancellationToken);

            await transactionContext.CommitAsync(cancellationToken);

            logger.LogDebug("Committed transaction for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction for {RequestName} failed. Rolling back.", requestName);
            throw;
        }
    }
}
