namespace ThePredictions.Contracts.Dashboard;

public class PendingMembersResultDto
{
    public bool IsAdminOfOpenLeague { get; init; }
    public List<PendingLeagueMemberDto> Members { get; init; } = [];
}
