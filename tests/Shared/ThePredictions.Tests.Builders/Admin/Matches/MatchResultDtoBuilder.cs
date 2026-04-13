using ThePredictions.Contracts.Admin.Matches;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Tests.Builders.Admin.Matches;

public class MatchResultDtoBuilder
{
    private int _matchId = 1;
    private int _homeScore = 2;
    private int _awayScore = 1;
    private MatchStatus _status = MatchStatus.Completed;

    public MatchResultDtoBuilder WithMatchId(int matchId)
    {
        _matchId = matchId;
        return this;
    }

    public MatchResultDtoBuilder WithHomeScore(int homeScore)
    {
        _homeScore = homeScore;
        return this;
    }

    public MatchResultDtoBuilder WithAwayScore(int awayScore)
    {
        _awayScore = awayScore;
        return this;
    }

    public MatchResultDtoBuilder WithStatus(MatchStatus status)
    {
        _status = status;
        return this;
    }

    public MatchResultDto Build() => new(_matchId, _homeScore, _awayScore, _status);
}
