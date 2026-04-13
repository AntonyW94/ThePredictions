using ThePredictions.Contracts.Admin.Matches;

namespace ThePredictions.Tests.Builders.Admin.Matches;

public class UpdateMatchRequestBuilder
{
    private int _homeTeamId = 1;
    private int _awayTeamId = 2;
    private DateTime _matchDateTimeUtc = new(2025, 6, 15, 15, 0, 0, DateTimeKind.Utc);

    public UpdateMatchRequestBuilder WithHomeTeamId(int homeTeamId)
    {
        _homeTeamId = homeTeamId;
        return this;
    }

    public UpdateMatchRequestBuilder WithAwayTeamId(int awayTeamId)
    {
        _awayTeamId = awayTeamId;
        return this;
    }

    public UpdateMatchRequestBuilder WithMatchDateTimeUtc(DateTime matchDateTimeUtc)
    {
        _matchDateTimeUtc = matchDateTimeUtc;
        return this;
    }

    public UpdateMatchRequest Build() => new()
    {
        HomeTeamId = _homeTeamId,
        AwayTeamId = _awayTeamId,
        MatchDateTimeUtc = _matchDateTimeUtc
    };
}
