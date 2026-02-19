using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Guards;
using ThePredictions.Domain.Services;

namespace ThePredictions.Application.Features.Predictions.Commands;

public class SubmitPredictionsCommandHandler(IRoundRepository roundRepository, IUserPredictionRepository userPredictionRepository, PredictionDomainService predictionDomainService) : IRequestHandler<SubmitPredictionsCommand>
{
    public async Task Handle(SubmitPredictionsCommand request, CancellationToken cancellationToken)
    {
        var round = await roundRepository.GetByIdAsync(request.RoundId, cancellationToken);

        Guard.Against.EntityNotFound(request.RoundId, round, "Round");

        var predictedScores = request.Predictions.Select(p => (p.MatchId, p.HomeScore, p.AwayScore));

        var predictions = predictionDomainService.SubmitPredictions(
            round,
            request.UserId,
            predictedScores);

        await userPredictionRepository.UpsertBatchAsync(predictions, cancellationToken);
    }
}