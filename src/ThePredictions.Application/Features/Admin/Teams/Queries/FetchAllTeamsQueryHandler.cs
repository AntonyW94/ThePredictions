using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Queries;

public class FetchAllTeamsQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<FetchAllTeamsQuery, IEnumerable<TeamDto>>
{
    public async Task<IEnumerable<TeamDto>> Handle(FetchAllTeamsQuery request, CancellationToken cancellationToken)
    {
        if (request.SeasonId.HasValue)
        {
            const string sql = @"
                SELECT DISTINCT
                    t.[Id],
                    t.[Name],
                    t.[ShortName],
                    t.[LogoUrl],
                    t.[Abbreviation],
                    t.[ApiTeamId]
                FROM
                    [Teams] t
                INNER JOIN
                    [Matches] m ON t.[Id] = m.[HomeTeamId] OR t.[Id] = m.[AwayTeamId]
                INNER JOIN
                    [Rounds] r ON m.[RoundId] = r.[Id]
                WHERE
                    r.[SeasonId] = @SeasonId
                ORDER BY
                    t.[Name] ASC";

            return await dbConnection.QueryAsync<TeamDto>(sql, cancellationToken, new { request.SeasonId });
        }

        const string allTeamsSql = @"
            SELECT
                [Id],
                [Name],
                [ShortName],
                [LogoUrl],
                [Abbreviation],
                [ApiTeamId]
            FROM [Teams]
            ORDER BY [Name] ASC";

        return await dbConnection.QueryAsync<TeamDto>(allTeamsSql, cancellationToken);
    }
}