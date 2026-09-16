using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;
using ThePredictions.Domain.Common;

namespace ThePredictions.Persistence.SqlServer.Data.Resilience;

/// <summary>
/// Recognises the failures that mean the database could not be reached, and tracks how long the
/// current outage has run.
/// </summary>
/// <remarks>
/// Registered as a singleton because the outage clock has to outlive the request that observed the
/// failure. State is in memory only, so a process restart during an outage restarts the clock - which
/// is the safe direction, as it delays the escalation to Error rather than causing a false one.
/// </remarks>
public class DatabaseOutageMonitor(
    IOptions<DatabaseAvailabilitySettings> settings,
    IDateTimeProvider dateTimeProvider) : IDatabaseOutageMonitor
{
    // Error numbers meaning the server was never reached: the network path failed, the connection was
    // dropped under us, or the provider gave up trying to open one. Deliberately narrower than
    // SqlTransientFaultDetector's set, which also carries deadlocks (1205) and command timeouts (-2).
    // Those are a statement that ran and lost, not a database that was absent, and they still say
    // something about our own code - so they stay in the Error bucket where a defect belongs.
    private static readonly HashSet<int> UnreachableErrorNumbers =
    [
        -1,    // General network error opening the connection
        40,    // Could not open a connection to SQL Server
        53,    // Network path was not found
        64,    // The specified network name is no longer available
        121,   // Semaphore timeout period has expired
        233,   // No process is on the other end of the pipe
        10053, // Transport-level error: connection aborted by the software in the host
        10054, // Connection reset by peer
        10060, // Connection attempt timed out
        10061  // Connection refused
    ];

    private readonly TimeSpan _recoveryGrace = TimeSpan.FromMinutes(settings.Value.OutageRecoveryGraceMinutes);

    private readonly object _gate = new();
    private DateTime? _outageStartedUtc;
    private DateTime? _lastFailureUtc;

    public bool IsDatabaseUnavailable(Exception exception)
    {
        if (ChainContains(exception, IsUnreachable))
            return true;

        // Pool exhaustion is ambiguous on its own: it is what an outage looks like from inside the app
        // once every pooled connection is stuck waiting, but it is equally what a connection leak looks
        // like against a perfectly healthy database. So it only counts while the database is already
        // known to be missing. That keeps a genuine leak in the Error bucket, where somebody needs to
        // see it, instead of quietly reclassifying our own defect as somebody else's outage.
        return ChainContains(exception, IsPoolExhaustion) && IsOutageInProgress();
    }

    public TimeSpan RecordFailure()
    {
        var nowUtc = dateTimeProvider.UtcNow;

        lock (_gate)
        {
            // A failure this long after the last one is a new outage, not a continuation. There is no
            // success signal to clear the clock with, and there does not need to be: a database that is
            // genuinely down produces a steady drip of failures from the per-minute jobs, so a quiet gap
            // means it came back.
            if (_lastFailureUtc is null || nowUtc - _lastFailureUtc.Value >= _recoveryGrace)
                _outageStartedUtc = nowUtc;

            _lastFailureUtc = nowUtc;

            return nowUtc - _outageStartedUtc!.Value;
        }
    }

    private bool IsOutageInProgress()
    {
        lock (_gate)
        {
            return _lastFailureUtc is not null && dateTimeProvider.UtcNow - _lastFailureUtc.Value < _recoveryGrace;
        }
    }

    // Walks the inner-exception chain, because the interesting exception is usually wrapped: a read
    // surfaces as ReadQueryFailedException, and the provider itself nests a Win32Exception under a
    // SqlException.
    private static bool ChainContains(Exception exception, Func<Exception, bool> predicate)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (predicate(current))
                return true;
        }

        return false;
    }

    private static bool IsUnreachable(Exception exception)
    {
        if (exception is not SqlException sqlException)
            return false;

        foreach (SqlError error in sqlException.Errors)
        {
            if (UnreachableErrorNumbers.Contains(error.Number))
                return true;
        }

        return false;
    }

    // Matched on the message because the provider reports pool exhaustion as a bare
    // InvalidOperationException with nothing else to key on. If the wording ever changes the match
    // fails safe: the exception stays unrecognised and is reported as an Error, which is what it was
    // before this class existed.
    private static bool IsPoolExhaustion(Exception exception) =>
        exception is InvalidOperationException
        && exception.Message.Contains("prior to obtaining a connection from the pool", StringComparison.Ordinal);
}
