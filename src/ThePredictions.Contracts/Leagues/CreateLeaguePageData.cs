using ThePredictions.Contracts.Admin.Seasons;

namespace ThePredictions.Contracts.Leagues;

public class CreateLeaguePageData
{
    public List<SeasonLookupDto> Seasons { get; init; } = [];
    public int DefaultPointsForExactScore { get; init; }
    public int DefaultPointsForCorrectResult { get; init; }
}