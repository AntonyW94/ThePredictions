using ThePredictions.Contracts.Predictions;

namespace ThePredictions.Tests.Builders.Predictions;

public class PredictionSubmissionDtoBuilder
{
    private int _matchId = 1;
    private int _homeScore = 2;
    private int _awayScore = 1;

    public PredictionSubmissionDtoBuilder WithMatchId(int matchId)
    {
        _matchId = matchId;
        return this;
    }

    public PredictionSubmissionDtoBuilder WithHomeScore(int homeScore)
    {
        _homeScore = homeScore;
        return this;
    }

    public PredictionSubmissionDtoBuilder WithAwayScore(int awayScore)
    {
        _awayScore = awayScore;
        return this;
    }

    public PredictionSubmissionDto Build() => new(_matchId, _homeScore, _awayScore);
}
