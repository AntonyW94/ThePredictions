using ThePredictions.Contracts.Boosts;
using ThePredictions.Domain.Models;
using ThePredictions.Domain.Services.Boosts;

namespace ThePredictions.Application.Repositories;

public interface IBoostReadRepository
{
    Task<(int SeasonId, int RoundNumber, DateTime DeadlineUtc)> GetRoundInfoAsync(int roundId, CancellationToken cancellationToken);
    Task<int?> GetLeagueSeasonIdAsync(int leagueId, CancellationToken cancellationToken);
    Task<IEnumerable<BoostDefinition>> GetBoostDefinitionsForLeagueAsync(int leagueId, CancellationToken cancellationToken);
    Task<bool> IsUserMemberOfLeagueAsync(string userId, int leagueId, CancellationToken cancellationToken);
    Task<LeagueBoostRuleSnapshot?> GetLeagueBoostRuleAsync(int leagueId, string boostCode, CancellationToken cancellationToken);
    Task<BoostUsageSnapshot> GetUserBoostUsageSnapshotAsync(string userId, int leagueId, int seasonId, int roundId, string boostCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserRoundBoostDto>> GetBoostsForRoundAsync(int roundId, CancellationToken cancellationToken);
}