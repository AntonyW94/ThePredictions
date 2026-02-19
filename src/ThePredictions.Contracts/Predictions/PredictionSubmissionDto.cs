namespace ThePredictions.Contracts.Predictions;

public record PredictionSubmissionDto(
    int MatchId,
    int HomeScore,
    int AwayScore
);