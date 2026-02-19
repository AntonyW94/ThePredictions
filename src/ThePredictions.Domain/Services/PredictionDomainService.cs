using Ardalis.GuardClauses;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Models;

namespace ThePredictions.Domain.Services;

public class PredictionDomainService(IDateTimeProvider dateTimeProvider)
{
    public IEnumerable<UserPrediction> SubmitPredictions(Round round, string userId, IEnumerable<(int MatchId, int HomeScore, int AwayScore)> predictedScores)
    {
        Guard.Against.Null(round);

        if (round.DeadlineUtc < dateTimeProvider.UtcNow)
            throw new InvalidOperationException("The deadline for submitting predictions for this round has passed.");

        var predictions = predictedScores.Select(p => UserPrediction.Create(
            userId,
            p.MatchId,
            p.HomeScore,
            p.AwayScore,
            dateTimeProvider
        )).ToList();

        return predictions;
    }
}
