using MediatR;
using ThePredictions.Application.Data;
using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public class GetMatchesForRoundQueryHandler(IApplicationReadDbConnection dbConnection)
    : IRequestHandler<GetMatchesForRoundQuery, IEnumerable<MatchInRoundDto>>
{
    public async Task<IEnumerable<MatchInRoundDto>> Handle(GetMatchesForRoundQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT
                m.[Id],
                m.[MatchDateTimeUtc],
                m.[HomeTeamId],
                ht.[Name] AS HomeTeamName,
                ht.[ShortName] AS HomeTeamShortName,
                ht.[Abbreviation] AS HomeTeamAbbreviation,
                ht.[LogoUrl] AS HomeTeamLogoUrl,
                m.[AwayTeamId],
                at.[Name] AS AwayTeamName,
                at.[ShortName] AS AwayTeamShortName,
                at.[Abbreviation] AS AwayTeamAbbreviation,
                at.[LogoUrl] AS AwayTeamLogoUrl,
                m.[ActualHomeTeamScore],
                m.[ActualAwayTeamScore],
                m.[Status]
            FROM 
                [Matches] m
            JOIN 
                [Teams] ht ON m.[HomeTeamId] = ht.[Id]
            JOIN 
                [Teams] at ON m.[AwayTeamId] = at.[Id]
            WHERE 
                m.[RoundId] = @RoundId
            ORDER BY 
                m.[MatchDateTimeUtc];";

        return await dbConnection.QueryAsync<MatchInRoundDto>(sql, cancellationToken, new { request.RoundId });
    }
}