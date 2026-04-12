namespace ThePredictions.Contracts.Dashboard;

public record PendingLeagueMemberDto(
    int LeagueId,
    string LeagueName,
    string UserId,
    string MemberName,
    DateTime JoinedAtUtc
);
