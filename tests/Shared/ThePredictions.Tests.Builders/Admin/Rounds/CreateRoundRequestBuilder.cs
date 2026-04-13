using ThePredictions.Contracts.Admin.Matches;
using ThePredictions.Contracts.Admin.Rounds;
using ThePredictions.Tests.Builders.Admin.Matches;

namespace ThePredictions.Tests.Builders.Admin.Rounds;

public class CreateRoundRequestBuilder
{
    private int _seasonId = 1;
    private int _roundNumber = 5;
    private DateTime _startDateUtc = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private DateTime _deadlineUtc = new(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc);
    private List<CreateMatchRequest> _matches = [new CreateMatchRequestBuilder().Build()];

    public CreateRoundRequestBuilder WithSeasonId(int seasonId)
    {
        _seasonId = seasonId;
        return this;
    }

    public CreateRoundRequestBuilder WithRoundNumber(int roundNumber)
    {
        _roundNumber = roundNumber;
        return this;
    }

    public CreateRoundRequestBuilder WithStartDateUtc(DateTime startDateUtc)
    {
        _startDateUtc = startDateUtc;
        return this;
    }

    public CreateRoundRequestBuilder WithDeadlineUtc(DateTime deadlineUtc)
    {
        _deadlineUtc = deadlineUtc;
        return this;
    }

    public CreateRoundRequestBuilder WithMatches(List<CreateMatchRequest> matches)
    {
        _matches = matches;
        return this;
    }

    public CreateRoundRequest Build() => new()
    {
        SeasonId = _seasonId,
        RoundNumber = _roundNumber,
        StartDateUtc = _startDateUtc,
        DeadlineUtc = _deadlineUtc,
        Matches = _matches
    };
}
