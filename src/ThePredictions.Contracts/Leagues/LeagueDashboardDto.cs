using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Contracts.Leagues;

public class LeagueDashboardDto
{
    public string LeagueName { get; init; } = string.Empty;
    public List<RoundDto> ViewableRounds { get; init; } = [];
}