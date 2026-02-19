using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Contracts.Leagues;

public record LeagueMemberDto(
    string UserId,
    string FullName, 
    DateTime JoinedAtUtc,
    LeagueMemberStatus Status);