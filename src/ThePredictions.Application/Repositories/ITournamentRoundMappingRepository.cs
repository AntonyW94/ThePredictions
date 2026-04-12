using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Repositories;

public interface ITournamentRoundMappingRepository
{
    Task<List<TournamentRoundMapping>> GetBySeasonIdAsync(int seasonId, CancellationToken cancellationToken);
    Task ReplaceAllForSeasonAsync(int seasonId, List<TournamentRoundMapping> mappings, CancellationToken cancellationToken);
}
