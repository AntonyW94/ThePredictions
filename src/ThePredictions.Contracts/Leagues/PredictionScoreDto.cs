using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Contracts.Leagues;

public record PredictionScoreDto(
    int MatchId,
    int? HomeScore,
    int? AwayScore,
    PredictionOutcome Outcome,
    bool IsHidden);