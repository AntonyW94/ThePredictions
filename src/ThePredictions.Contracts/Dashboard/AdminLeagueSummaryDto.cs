namespace ThePredictions.Contracts.Dashboard;

public record AdminLeagueSummaryDto(
    int LeagueId,
    string LeagueName,
    DateTime EntryDeadlineUtc,
    int MemberCount,
    int PendingCount
);
