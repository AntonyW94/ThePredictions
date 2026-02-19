using ThePredictions.Domain.Common;

namespace ThePredictions.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
