using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Contracts.Admin.Rounds;

public record RoundDto(
    int Id,
    int SeasonId,
    int RoundNumber,
    string? ApiRoundName,
    DateTime StartDateUtc,
    DateTime DeadlineUtc,
    RoundStatus Status,
    int MatchCount);