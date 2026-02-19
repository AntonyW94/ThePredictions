using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Contracts.Dashboard;

public record LeagueRequestDto(
    int LeagueId,
    string LeagueName,
    string SeasonName,
    LeagueMemberStatus Status,
    DateTime JoinedAtUtc,    
    string AdminName,
    int MemberCount,
    decimal EntryFee,
    decimal PotValue
);