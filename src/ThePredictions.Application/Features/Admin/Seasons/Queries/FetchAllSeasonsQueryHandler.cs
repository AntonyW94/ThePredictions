using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Seasons;

namespace ThePredictions.Application.Features.Admin.Seasons.Queries;

public class FetchAllSeasonsQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<FetchAllSeasonsQuery, IEnumerable<SeasonDto>>
{
    public async Task<IEnumerable<SeasonDto>> Handle(FetchAllSeasonsQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT
                s.[Id],
                s.[Name],
                s.[StartDateUtc],
                s.[EndDateUtc],
                s.[IsActive],
                s.[NumberOfRounds],
                (SELECT COUNT(*) FROM [Rounds] r WHERE r.[SeasonId] = s.[Id]) as RoundCount
            FROM
                [Seasons] s
            ORDER BY
                s.[StartDateUtc] DESC;";

        return await dbConnection.QueryAsync<SeasonDto>(sql, cancellationToken);
    }
}