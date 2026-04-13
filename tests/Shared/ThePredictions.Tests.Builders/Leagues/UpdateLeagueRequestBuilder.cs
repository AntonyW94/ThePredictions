using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Tests.Builders.Leagues;

public class UpdateLeagueRequestBuilder
{
    private string _name = "Updated League";
    private decimal _price = 10.00m;
    private DateTime _entryDeadlineUtc = new(2099, 6, 1, 12, 0, 0, DateTimeKind.Utc);
    private int _pointsForExactScore = 3;
    private int _pointsForCorrectResult = 1;

    public UpdateLeagueRequestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UpdateLeagueRequestBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public UpdateLeagueRequestBuilder WithEntryDeadlineUtc(DateTime entryDeadlineUtc)
    {
        _entryDeadlineUtc = entryDeadlineUtc;
        return this;
    }

    public UpdateLeagueRequestBuilder WithPointsForExactScore(int pointsForExactScore)
    {
        _pointsForExactScore = pointsForExactScore;
        return this;
    }

    public UpdateLeagueRequestBuilder WithPointsForCorrectResult(int pointsForCorrectResult)
    {
        _pointsForCorrectResult = pointsForCorrectResult;
        return this;
    }

    public UpdateLeagueRequest Build() => new()
    {
        Name = _name,
        Price = _price,
        EntryDeadlineUtc = _entryDeadlineUtc,
        PointsForExactScore = _pointsForExactScore,
        PointsForCorrectResult = _pointsForCorrectResult
    };
}
