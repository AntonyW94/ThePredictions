using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Queries;

public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamDto?>
{
    private readonly IApplicationReadDbConnection _dbConnection;

    public GetTeamByIdQueryHandler(IApplicationReadDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

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

        return await _dbConnection.QuerySingleOrDefaultAsync<TeamDto>(sql, cancellationToken, new { request.Id });
    }
}