using ThePredictions.Contracts.Admin.Users;
using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Services;

public interface IReminderService
{
    Task<bool> ShouldSendReminderAsync(Round round, DateTime nowUtc);
    Task<List<ChaseUserDto>> GetUsersMissingPredictionsAsync(int roundId, CancellationToken cancellationToken);
}