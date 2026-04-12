using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Repositories;

public interface ISeasonRepository
{
    #region Create

    Task<Season> CreateAsync(Season season, CancellationToken cancellationToken);

    #endregion

    #region Read

    Task<Season?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Season>> GetActiveSeasonsAsync(CancellationToken cancellationToken);
    Task<bool> HasPredictionsAsync(int seasonId, CancellationToken cancellationToken);

    #endregion

    #region Update

    Task UpdateAsync(Season request, CancellationToken cancellationToken);

    #endregion

    #region Delete

    Task DeleteAsync(int seasonId, CancellationToken cancellationToken);

    #endregion
}