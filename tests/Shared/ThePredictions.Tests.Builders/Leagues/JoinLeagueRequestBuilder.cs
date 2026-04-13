using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Tests.Builders.Leagues;

public class JoinLeagueRequestBuilder
{
    private string _entryCode = "ABC123";

    public JoinLeagueRequestBuilder WithEntryCode(string entryCode)
    {
        _entryCode = entryCode;
        return this;
    }

    public JoinLeagueRequest Build() => new()
    {
        EntryCode = _entryCode
    };
}
