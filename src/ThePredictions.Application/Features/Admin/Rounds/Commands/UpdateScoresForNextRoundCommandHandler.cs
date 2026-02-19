using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Admin.Matches;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public class UpdateScoresForNextRoundCommandHandler(
    IRoundRepository roundRepository,
    IFootballDataService footballDataService,
    IMediator mediator) : IRequestHandler<UpdateScoresForNextRoundCommand>
{
    public async Task Handle(UpdateScoresForNextRoundCommand request, CancellationToken cancellationToken)
    {
        var activeRound = await roundRepository.GetOldestInProgressRoundAsync(request.SeasonId, cancellationToken);
        if (activeRound == null || !activeRound.Matches.Any())
            return;

        var matchesToCheck = activeRound.Matches
            .Where(m => m.MatchDateTimeUtc < DateTime.UtcNow && m.Status != MatchStatus.Completed)
            .ToList();

        if (!matchesToCheck.Any())
            return;
        
        var externalIds = matchesToCheck
            .Where(m => m.ExternalId.HasValue)
            .Select(m => m.ExternalId.GetValueOrDefault())
            .ToList();

        if (!externalIds.Any())
            return;

        var liveFixtures = (await footballDataService.GetFixturesByIdsAsync(externalIds, cancellationToken)).ToList();
        if (!liveFixtures.Any()) 
            return;

        var matchResults = liveFixtures.Where(f => f.Fixture != null && f.Goals != null).Select(fixture =>
        {
            var localMatch = activeRound.Matches.First(m => m.ExternalId == fixture.Fixture!.Id);
            return new MatchResultDto(
                localMatch.Id,
                fixture.Goals!.Home.GetValueOrDefault(),
                fixture.Goals.Away.GetValueOrDefault(),
                GetMatchStatus(fixture.Fixture!.Status.Short)
            );
        }).ToList();

        if (matchResults.Any())
        {
            var updateCommand = new UpdateMatchResultsCommand(activeRound.Id, matchResults);
            await mediator.Send(updateCommand, cancellationToken);
        }
    }

    private static MatchStatus GetMatchStatus(string apiStatus) => apiStatus switch
    {
        "FT" => MatchStatus.Completed,
        "HT" or "1H" or "2H" => MatchStatus.InProgress,
        _ => MatchStatus.Scheduled
    };
}