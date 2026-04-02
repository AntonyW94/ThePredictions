namespace ThePredictions.Contracts.Admin.Seasons;

public record SeasonDto(
    int Id,
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    bool IsActive,
    int NumberOfRounds,
    int CompetitionType,
    int RoundCount
) : SeasonLookupDto(Id, Name, StartDateUtc);