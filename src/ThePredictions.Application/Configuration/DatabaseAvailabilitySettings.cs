using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Application.Configuration;

[ExcludeFromCodeCoverage(Justification = "Options type bound from configuration: properties only, no logic to test.")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class DatabaseAvailabilitySettings
{
    public const string SectionName = "DatabaseAvailability";

    // How long the database has to have been unreachable before a failed request is reported as an
    // Error rather than Information. The hosting is a shared instance where short outages are part of
    // the contract and nobody can act on one, so by ADR-0018 a brief outage is not an Error: it is
    // recorded in full and left out of alerting. Past this point the assumption that it will fix
    // itself has stopped being reasonable, and somebody does need to look.
    public int OutageErrorThresholdMinutes { get; init; } = 60;

    // A failure arriving at least this long after the previous one is treated as the start of a new
    // outage rather than a continuation of the old one. This is how the outage clock resets without
    // needing a success to be reported: the every-minute score-update job means a genuine outage
    // produces failures roughly a minute apart, so a gap this size means the database came back.
    //
    // Raising it risks two separate outages being counted as one long one and reaching Error early.
    // Lowering it below the interval of the most frequent database-touching job would restart the
    // clock during a real outage, so it could never reach the threshold at all.
    public int OutageRecoveryGraceMinutes { get; init; } = 5;
}
