using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Queries;

public class FetchAllTeamsQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<FetchAllTeamsQuery, IEnumerable<TeamDto>>
{
    public async Task<IEnumerable<TeamDto>> Handle(FetchAllTeamsQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT
                [Id],
                [Name],
                [ShortName],
                [LogoUrl],
                [Abbreviation],
                [ApiTeamId]
            FROM [Teams]
            ORDER BY [Name] ASC";

        return await dbConnection.QueryAsync<TeamDto>(sql, cancellationToken);
    }
}