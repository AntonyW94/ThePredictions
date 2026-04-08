namespace ThePredictions.Contracts.Homepage;

public record HomepageSeasonDto(
    int Id,
    string Name,
    int CompetitionType,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    int IsInProgress,
    int IsUpcoming,
    int LeagueCount,
    int PlayerCount,
    decimal TotalPrizeFund
);
