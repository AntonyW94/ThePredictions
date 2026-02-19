using MediatR;
using ThePredictions.Application.Features.Admin.Rounds.Commands;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services.Boosts;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public class RecalculateSeasonStatsCommandHandler : IRequestHandler<RecalculateSeasonStatsCommand>
{
    private readonly IRoundRepository _roundRepository;
    private readonly ILeagueRepository _leagueRepository;
    private readonly IBoostService _boostService;
    private readonly IMediator _mediator;

    public RecalculateSeasonStatsCommandHandler(
        IRoundRepository roundRepository,
        ILeagueRepository leagueRepository,
        IBoostService boostService,
        IMediator mediator)
    {
        _roundRepository = roundRepository;
        _leagueRepository = leagueRepository;
        _boostService = boostService;
        _mediator = mediator;
    }

    public async Task Handle(RecalculateSeasonStatsCommand request, CancellationToken cancellationToken)
    {
        var rounds = (await _roundRepository.GetAllForSeasonAsync(request.SeasonId, cancellationToken)).Values.ToList();

        var completedRounds = rounds
            .Where(r => r.Status == RoundStatus.Completed)
            .OrderBy(r => r.StartDateUtc)
            .ToList();

        foreach (var round in completedRounds)
        {
            await _roundRepository.UpdateRoundResultsAsync(round.Id, cancellationToken);
            await _leagueRepository.UpdateLeagueRoundResultsAsync(round.Id, cancellationToken);
            await _boostService.ApplyRoundBoostsAsync(round.Id, cancellationToken);

            var leagueIds = await _leagueRepository.GetLeagueIdsForSeasonAsync(round.SeasonId, cancellationToken);

            foreach (var leagueId in leagueIds)
            {
                var processPrizesCommand = new ProcessPrizesCommand
                {
                    RoundId = round.Id,
                    LeagueId = leagueId
                };

                await _mediator.Send(processPrizesCommand, cancellationToken);
            }
        }
    }
}