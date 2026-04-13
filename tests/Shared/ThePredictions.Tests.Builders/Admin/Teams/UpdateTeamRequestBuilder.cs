using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Tests.Builders.Admin.Teams;

public class UpdateTeamRequestBuilder
{
    private string _name = "Manchester United";
    private string _shortName = "Man United";
    private string _logoUrl = "https://example.com/logo.png";
    private string _abbreviation = "MUN";

    public UpdateTeamRequestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UpdateTeamRequestBuilder WithShortName(string shortName)
    {
        _shortName = shortName;
        return this;
    }

    public UpdateTeamRequestBuilder WithLogoUrl(string logoUrl)
    {
        _logoUrl = logoUrl;
        return this;
    }

    public UpdateTeamRequestBuilder WithAbbreviation(string abbreviation)
    {
        _abbreviation = abbreviation;
        return this;
    }

    public UpdateTeamRequest Build() => new()
    {
        Name = _name,
        ShortName = _shortName,
        LogoUrl = _logoUrl,
        Abbreviation = _abbreviation
    };
}
