using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Seasons;
using ThePredictions.Contracts.Leagues;
using ThePredictions.Domain.Common.Constants;

namespace ThePredictions.Application.Features.Leagues.Queries;

public class GetCreateLeaguePageDataQueryHandler(IApplicationReadDbConnection dbConnection) : IRequestHandler<GetCreateLeaguePageDataQuery, CreateLeaguePageData>
{
    public async Task<CreateLeaguePageData> Handle(GetCreateLeaguePageDataQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT
                s.[Id],
                s.[Name],
                s.[StartDateUtc]
            FROM [Seasons] s
            WHERE s.[IsActive] = 1
            ORDER BY s.[StartDateUtc] DESC;";

        var seasons = await dbConnection.QueryAsync<SeasonLookupDto>(sql, cancellationToken);

        return new CreateLeaguePageData
        {
            Seasons = seasons.ToList(),
            DefaultPointsForExactScore = PublicLeagueSettings.PointsForExactScore,
            DefaultPointsForCorrectResult = PublicLeagueSettings.PointsForCorrectResult
        };
    }
}