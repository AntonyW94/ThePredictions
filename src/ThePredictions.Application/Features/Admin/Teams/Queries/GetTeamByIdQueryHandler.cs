using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Queries;

public class GetTeamByIdQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<GetTeamByIdQuery, TeamDto?>
{
    public async Task<TeamDto?> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
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
            WHERE [Id] = @Id";

        return await dbConnection.QuerySingleOrDefaultAsync<TeamDto>(sql, cancellationToken, new { request.Id });
    }
}