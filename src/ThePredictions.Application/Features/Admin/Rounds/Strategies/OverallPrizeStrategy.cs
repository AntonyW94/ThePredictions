using ThePredictions.Application.Common.Helpers;
using ThePredictions.Application.Features.Admin.Rounds.Commands;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Features.Admin.Rounds.Strategies;

public class OverallPrizeStrategy(
    IWinningsRepository winningsRepository,
    IRoundRepository roundRepository,
    ILeagueRepository leagueRepository,
    IDateTimeProvider dateTimeProvider) : IPrizeStrategy
{
    public PrizeType PrizeType => PrizeType.Overall;

    public async Task AwardPrizes(ProcessPrizesCommand command, CancellationToken cancellationToken)
    {
        var currentRound = await roundRepository.GetByIdAsync(command.RoundId, cancellationToken);
        if (currentRound == null)
            return;

        var isLastRoundOfSeason = await roundRepository.IsLastRoundOfSeasonAsync(currentRound.Id, currentRound.SeasonId, cancellationToken);
        if (!isLastRoundOfSeason)
            return;

        var league = await leagueRepository.GetByIdWithAllDataAsync(command.LeagueId, cancellationToken);
        if (league == null)
            return;

        var overallPrizeSettings = league.PrizeSettings
            .Where(p => p.PrizeType == PrizeType.Overall)
            .OrderBy(p => p.Rank)
            .ToList();

        if (!overallPrizeSettings.Any()) return;

        await winningsRepository.DeleteWinningsForOverallAsync(league.Id, cancellationToken);

        var overallRankings = league.GetOverallRankings();
        if (!overallRankings.Any())
            return;

        var allNewWinnings = new List<Winning>();

        foreach (var prizeSetting in overallPrizeSettings)
        {
            var rankingGroup = overallRankings.FirstOrDefault(r => r.Rank == prizeSetting.Rank);
            if (rankingGroup == null || !rankingGroup.Members.Any())
                continue;
            
            var winnersForThisRank = rankingGroup.Members;

            var individualPrizes = PrizeDistributionHelper.DistributePrizeMoney(
                prizeSetting.PrizeAmount,
                winnersForThisRank.Count
            );

            for (var i = 0; i < winnersForThisRank.Count; i++)
            {
                var winner = winnersForThisRank[i];
                var prizeAmount = individualPrizes[i]; 

                var newWinning = Winning.Create(
                    winner.UserId,
                    prizeSetting.Id,
                    prizeAmount,
                    null,
                    null,
                    dateTimeProvider
                );
                allNewWinnings.Add(newWinning);
            }
        }

        if (allNewWinnings.Any())
            await winningsRepository.AddWinningsAsync(allNewWinnings, cancellationToken);
    }
}