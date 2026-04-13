using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Tests.Builders.Leagues;

public class CreateLeagueRequestBuilder
{
    private string _name = "Test League";
    private int _seasonId = 1;
    private decimal _price = 10.00m;
    private DateTime _entryDeadlineUtc = new(2099, 6, 1, 12, 0, 0, DateTimeKind.Utc);
    private int _pointsForExactScore = 3;
    private int _pointsForCorrectResult = 1;

    public CreateLeagueRequestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CreateLeagueRequestBuilder WithSeasonId(int seasonId)
    {
        _seasonId = seasonId;
        return this;
    }

    public CreateLeagueRequestBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public CreateLeagueRequestBuilder WithEntryDeadlineUtc(DateTime entryDeadlineUtc)
    {
        _entryDeadlineUtc = entryDeadlineUtc;
        return this;
    }

    public CreateLeagueRequestBuilder WithPointsForExactScore(int pointsForExactScore)
    {
        _pointsForExactScore = pointsForExactScore;
        return this;
    }

    public CreateLeagueRequestBuilder WithPointsForCorrectResult(int pointsForCorrectResult)
    {
        _pointsForCorrectResult = pointsForCorrectResult;
        return this;
    }

    public CreateLeagueRequest Build() => new()
    {
        Name = _name,
        SeasonId = _seasonId,
        Price = _price,
        EntryDeadlineUtc = _entryDeadlineUtc,
        PointsForExactScore = _pointsForExactScore,
        PointsForCorrectResult = _pointsForCorrectResult
    };
}
