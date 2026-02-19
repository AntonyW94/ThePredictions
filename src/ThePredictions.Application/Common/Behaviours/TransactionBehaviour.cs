using MediatR;
using Microsoft.Extensions.Logging;
using ThePredictions.Application.Common.Interfaces;
using System.Transactions;

namespace ThePredictions.Application.Common.Behaviours;

public class TransactionBehaviour<TRequest, TResponse>(ILogger<TransactionBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            logger.LogDebug("Beginning transaction for {RequestName}", requestName);

            var response = await next(cancellationToken);

            scope.Complete();

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