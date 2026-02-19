using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Guards;
using ThePredictions.Domain.Services;

namespace ThePredictions.Application.Features.Predictions.Commands;

public class SubmitPredictionsCommandHandler : IRequestHandler<SubmitPredictionsCommand>
{
    private readonly IRoundRepository _roundRepository;
    private readonly IUserPredictionRepository _userPredictionRepository;
    private readonly PredictionDomainService _predictionDomainService;

    public SubmitPredictionsCommandHandler(IRoundRepository roundRepository, IUserPredictionRepository userPredictionRepository, PredictionDomainService predictionDomainService)
    {
        _roundRepository = roundRepository;
        _userPredictionRepository = userPredictionRepository;
        _predictionDomainService = predictionDomainService;
    }

    public async Task Handle(SubmitPredictionsCommand request, CancellationToken cancellationToken)
    {
        var round = await _roundRepository.GetByIdAsync(request.RoundId, cancellationToken);

        Guard.Against.EntityNotFound(request.RoundId, round, "Round");

        var predictedScores = request.Predictions.Select(p => (p.MatchId, p.HomeScore, p.AwayScore));

        var predictions = _predictionDomainService.SubmitPredictions(
            round,
            request.UserId,
            predictedScores);

        await _userPredictionRepository.UpsertBatchAsync(predictions, cancellationToken);
    }
}