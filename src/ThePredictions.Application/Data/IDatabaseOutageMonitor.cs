namespace ThePredictions.Application.Data;

/// <summary>
/// Recognises the failures that mean the database could not be reached, and tracks how long the
/// current outage has been going on, so a caller can report a brief one differently from a lasting one.
/// </summary>
public interface IDatabaseOutageMonitor
{
    /// <summary>
    /// Whether <paramref name="exception"/> means the database was unreachable, rather than that a
    /// statement ran and failed. Pure: call <see cref="RecordFailure"/> to actually record it.
    /// </summary>
    bool IsDatabaseUnavailable(Exception exception);

    /// <summary>
    /// Records a failure at the current time and returns how long the outage it belongs to has lasted.
    /// The first failure of an outage returns <see cref="TimeSpan.Zero"/>.
    /// </summary>
    TimeSpan RecordFailure();
}
