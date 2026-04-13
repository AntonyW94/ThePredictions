using ThePredictions.Contracts.Admin.Matches;
using ThePredictions.Contracts.Admin.Rounds;
using ThePredictions.Tests.Builders.Admin.Matches;

namespace ThePredictions.Tests.Builders.Admin.Rounds;

public class UpdateRoundRequestBuilder
{
    private DateTime _startDateUtc = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private DateTime _deadlineUtc = new(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc);
    private List<UpdateMatchRequest> _matches = [new UpdateMatchRequestBuilder().Build()];

    public UpdateRoundRequestBuilder WithStartDateUtc(DateTime startDateUtc)
    {
        _startDateUtc = startDateUtc;
        return this;
    }

    public UpdateRoundRequestBuilder WithDeadlineUtc(DateTime deadlineUtc)
    {
        _deadlineUtc = deadlineUtc;
        return this;
    }

    public UpdateRoundRequestBuilder WithMatches(List<UpdateMatchRequest> matches)
    {
        _matches = matches;
        return this;
    }

    public UpdateRoundRequest Build() => new()
    {
        StartDateUtc = _startDateUtc,
        DeadlineUtc = _deadlineUtc,
        Matches = _matches
    };
}
