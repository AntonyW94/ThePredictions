using ThePredictions.Contracts.Boosts;

namespace ThePredictions.Tests.Builders.Boosts;

public class ApplyBoostRequestBuilder
{
    private int _leagueId = 1;
    private int _roundId = 1;
    private string _boostCode = "DOUBLE";

    public ApplyBoostRequestBuilder WithLeagueId(int leagueId)
    {
        _leagueId = leagueId;
        return this;
    }

    public ApplyBoostRequestBuilder WithRoundId(int roundId)
    {
        _roundId = roundId;
        return this;
    }

    public ApplyBoostRequestBuilder WithBoostCode(string boostCode)
    {
        _boostCode = boostCode;
        return this;
    }

    public ApplyBoostRequest Build() => new(_leagueId, _roundId, _boostCode);
}
