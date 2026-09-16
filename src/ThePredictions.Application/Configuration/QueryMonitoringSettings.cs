using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Application.Configuration;

[ExcludeFromCodeCoverage(Justification = "Options type bound from configuration: properties only, no logic to test.")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class QueryMonitoringSettings
{
    // Read queries whose execution meets or exceeds this many milliseconds are logged at Warning
    // level, so slow database paths surface in the logs without a full APM setup. Default 500ms.
    //
    // This is measured against the query alone, with the cost of getting a connection taken off
    // first - that half has its own threshold below. Measuring the two together is what made the
    // warning misleading: over a fortnight of production logs, 73% of the reads it named as slow
    // queries had spent nearly all of their time acquiring a connection and only 2-40ms querying,
    // and the obvious reading of a "Slow query" warning - that the SQL wants an index - was the
    // wrong conclusion for every one of them.
    public int SlowQueryThresholdMilliseconds { get; init; } = 500;

    // Reads that wait at least this many milliseconds for a connection are logged at Warning level,
    // separately from the query itself. A warm pool hands one back in single-digit milliseconds, so
    // anything at this scale means a fresh physical connection was opened - a TCP connect, a TLS
    // handshake and a SQL login to a remote host - or that the pool was exhausted. Neither is fixed
    // by touching the SQL, which is why this no longer counts towards the query threshold.
    public int SlowConnectionThresholdMilliseconds { get; init; } = 500;

    // Transactional commands whose transaction stays open for at least this many milliseconds are
    // logged at Warning level. With READ_COMMITTED_SNAPSHOT off, a reader waits for whichever writer
    // holds the rows it wants, so the duration a transaction is held open is the duration unrelated
    // reads can be blocked for - which makes it the number to watch, not just the command's own cost.
    // Deliberately higher than the read threshold: a write transaction is expected to be slower than
    // a read, and the interesting case is one held open far longer than the reads it delays.
    public int SlowTransactionThresholdMilliseconds { get; init; } = 1000;
}
