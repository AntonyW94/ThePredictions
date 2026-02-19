using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Seasons;

namespace ThePredictions.Application.Features.Admin.Seasons.Queries;

public class GetSeasonByIdQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<GetSeasonByIdQuery, SeasonDto?>
{
    public async Task<SeasonDto?> Handle(GetSeasonByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT
                s.[Id],
                s.[Name],
                s.[StartDateUtc],
                s.[EndDateUtc],
                s.[IsActive],
                s.[NumberOfRounds],
                (SELECT COUNT(*) FROM [Rounds] r WHERE r.[SeasonId] = s.[Id]) as 'RoundCount'
            FROM
                [Seasons] s
            WHERE
                s.[Id] = @Id";

        return await dbConnection.QuerySingleOrDefaultAsync<SeasonDto>(sql, cancellationToken, new { request.Id });
    }
}