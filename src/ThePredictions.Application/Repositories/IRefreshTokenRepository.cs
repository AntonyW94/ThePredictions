using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Repositories;

public interface IRefreshTokenRepository
{
    #region Create

    Task CreateAsync(RefreshToken token, CancellationToken cancellationToken);

    #endregion

    #region Read

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);

    #endregion

    #region Update

    Task RevokeAllForUserAsync(string userId, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken); 

    #endregion
}