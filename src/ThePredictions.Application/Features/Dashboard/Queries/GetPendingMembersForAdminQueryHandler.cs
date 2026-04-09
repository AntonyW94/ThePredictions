using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Dashboard;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public class GetPendingMembersForAdminQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<GetPendingMembersForAdminQuery, PendingMembersResultDto>
{
    public async Task<PendingMembersResultDto> Handle(
        GetPendingMembersForAdminQuery request,
        CancellationToken cancellationToken)
    {
        const string adminCheckSql = @"
            SELECT
                COUNT(*)
            FROM
                [Leagues] l
            WHERE
                l.[AdministratorId] = @UserId
                AND l.[EntryDeadlineUtc] >= GETUTCDATE()";

        var adminLeagueCount = await dbConnection.QuerySingleOrDefaultAsync<int>(
            adminCheckSql, cancellationToken, new { request.UserId });

        if (adminLeagueCount == 0)
        {
            return new PendingMembersResultDto
            {
                IsAdminOfOpenLeague = false
            };
        }

        const string membersSql = @"
            SELECT
                l.[Id] AS LeagueId,
                l.[Name] AS LeagueName,
                lm.[UserId],
                u.[FirstName] + ' ' + LEFT(u.[LastName], 1) AS MemberName,
                lm.[JoinedAtUtc]
            FROM
                [LeagueMembers] lm
            JOIN
                [Leagues] l ON lm.[LeagueId] = l.[Id]
            JOIN
                [AspNetUsers] u ON lm.[UserId] = u.[Id]
            WHERE
                l.[AdministratorId] = @UserId
                AND l.[EntryDeadlineUtc] >= GETUTCDATE()
                AND lm.[Status] = @PendingStatus
            ORDER BY
                l.[Name],
                lm.[JoinedAtUtc]";

        var members = await dbConnection.QueryAsync<PendingLeagueMemberDto>(
            membersSql,
            cancellationToken,
            new
            {
                request.UserId,
                PendingStatus = nameof(LeagueMemberStatus.Pending)
            });

        return new PendingMembersResultDto
        {
            IsAdminOfOpenLeague = true,
            Members = members.ToList()
        };
    }
}
