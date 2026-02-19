using ThePredictions.Application.Common.Helpers;
using ThePredictions.Application.Features.Admin.Rounds.Commands;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Features.Admin.Rounds.Strategies;

public class RoundPrizeStrategy : IPrizeStrategy
{
    private readonly IWinningsRepository _winningsRepository;
    private readonly IRoundRepository _roundRepository;
    private readonly ILeagueRepository _leagueRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RoundPrizeStrategy(
        IWinningsRepository winningsRepository,
        IRoundRepository roundRepository,
        ILeagueRepository leagueRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _winningsRepository = winningsRepository;
        _roundRepository = roundRepository;
        _leagueRepository = leagueRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public PrizeType PrizeType => PrizeType.Round;

    public async Task AwardPrizes(ProcessPrizesCommand command, CancellationToken cancellationToken)
    {
        var round = await _roundRepository.GetByIdAsync(command.RoundId, cancellationToken);
        if (round == null)
            return;
        
        var league = await _leagueRepository.GetByIdWithAllDataAsync(command.LeagueId, cancellationToken);

        var roundPrize = league?.PrizeSettings.FirstOrDefault(p => p.PrizeType == PrizeType.Round);
        if (roundPrize == null)
            return;

        if (league != null)
        {
            await _winningsRepository.DeleteWinningsForRoundAsync(league.Id, round.RoundNumber, cancellationToken);

            var roundWinners = league.GetRoundWinners(round.Id);
            if (!roundWinners.Any())
                return;

            var individualPrizes = PrizeDistributionHelper.DistributePrizeMoney(
                roundPrize.PrizeAmount,
                roundWinners.Count
            );

            var allNewWinnings = new List<Winning>();

            for (var i = 0; i < roundWinners.Count; i++)
            {
                var winner = roundWinners[i];
                var prizeAmount = individualPrizes[i];

                var newWinning = Winning.Create(
                    winner.UserId,
                    roundPrize.Id,
                    prizeAmount,
                    round.RoundNumber,
                    null,
                    _dateTimeProvider
                );
                allNewWinnings.Add(newWinning);
            }


            await _winningsRepository.AddWinningsAsync(allNewWinnings, cancellationToken);
        }
    }
}