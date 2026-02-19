using ThePredictions.Domain.Common;

namespace ThePredictions.Tests.Shared.Helpers;

public class TestDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = utcNow;

    public void AdvanceBy(TimeSpan duration)
    {
        UtcNow = UtcNow.Add(duration);
    }
}
