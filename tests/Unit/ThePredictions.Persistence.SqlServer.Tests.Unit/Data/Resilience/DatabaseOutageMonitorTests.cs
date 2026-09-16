using FluentAssertions;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Persistence.SqlServer.Data.Resilience;
using ThePredictions.Tests.Shared.Helpers;
using Xunit;

namespace ThePredictions.Persistence.SqlServer.Tests.Unit.Data.Resilience;

/// <summary>
/// The monitor decides two separate things: whether a failure means the database was absent, and how long
/// it has been absent for. Together they decide whether a failed request is filed at Information or alerted
/// on as an Error, so both halves are pinned here.
/// </summary>
public class DatabaseOutageMonitorTests
{
    private readonly TestDateTimeProvider _dateTimeProvider = new(new DateTime(2026, 9, 16, 4, 0, 0, DateTimeKind.Utc));

    private DatabaseOutageMonitor BuildMonitor(int recoveryGraceMinutes = 5) =>
        new(Options.Create(new DatabaseAvailabilitySettings { OutageRecoveryGraceMinutes = recoveryGraceMinutes }),
            _dateTimeProvider);

    [Fact]
    public void IsDatabaseUnavailable_ShouldBeTrue_WhenTheServerCouldNotBeReached()
    {
        // 64 is what the September 2026 outages actually carried: "The specified network name is no longer
        // available", both environments at once, because the shared instance had gone.
        var exception = SqlExceptionFactory.WithErrorNumbers(64);

        BuildMonitor().IsDatabaseUnavailable(exception).Should().BeTrue();
    }

    [Fact]
    public void IsDatabaseUnavailable_ShouldBeTrue_WhenTheConnectionFaultIsWrapped()
    {
        // A read surfaces as ReadQueryFailedException, so the connection fault is never the outer exception.
        var wrapped = new InvalidOperationException("A read query failed.", SqlExceptionFactory.WithErrorNumbers(53));

        BuildMonitor().IsDatabaseUnavailable(wrapped).Should().BeTrue();
    }

    // A deadlock and a command timeout are a statement that ran and lost, not a database that was missing.
    // They say something about our own code, so they must stay in the Error bucket.
    [Theory]
    [InlineData(1205)] // Deadlock victim
    [InlineData(-2)]   // Command timeout
    public void IsDatabaseUnavailable_ShouldBeFalse_WhenTheStatementRanAndFailed(int errorNumber)
    {
        BuildMonitor().IsDatabaseUnavailable(SqlExceptionFactory.WithErrorNumbers(errorNumber)).Should().BeFalse();
    }

    [Fact]
    public void IsDatabaseUnavailable_ShouldBeFalse_WhenTheExceptionIsNotADatabaseFault()
    {
        BuildMonitor().IsDatabaseUnavailable(new FormatException("Input string was not in a correct format.")).Should().BeFalse();
    }

    // Pool exhaustion against a healthy database is a connection leak - our defect - so on its own it must
    // not be reclassified as somebody else's outage.
    [Fact]
    public void IsDatabaseUnavailable_ShouldBeFalse_WhenThePoolIsExhaustedAndTheDatabaseIsHealthy()
    {
        BuildMonitor().IsDatabaseUnavailable(PoolExhaustion()).Should().BeFalse();
    }

    // The same exception during a known outage is a symptom of it, and reporting it as a defect is what put
    // an Error in the channel for every outage.
    [Fact]
    public void IsDatabaseUnavailable_ShouldBeTrue_WhenThePoolIsExhaustedDuringAnOutage()
    {
        var monitor = BuildMonitor();
        monitor.IsDatabaseUnavailable(SqlExceptionFactory.WithErrorNumbers(64));
        monitor.RecordFailure();

        monitor.IsDatabaseUnavailable(PoolExhaustion()).Should().BeTrue();
    }

    [Fact]
    public void RecordFailure_ShouldReportNoElapsedTime_ForTheFirstFailureOfAnOutage()
    {
        BuildMonitor().RecordFailure().Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void RecordFailure_ShouldAccumulate_WhileFailuresKeepArriving()
    {
        var monitor = BuildMonitor();

        monitor.RecordFailure();
        _dateTimeProvider.AdvanceBy(TimeSpan.FromMinutes(1));
        monitor.RecordFailure();
        _dateTimeProvider.AdvanceBy(TimeSpan.FromMinutes(1));

        monitor.RecordFailure().Should().Be(TimeSpan.FromMinutes(2));
    }

    // The whole point of the escalation: an outage that genuinely persists has to reach the threshold, so a
    // steady drip of failures a minute apart must not keep restarting the clock.
    [Fact]
    public void RecordFailure_ShouldReachAnHour_WhenTheDatabaseStaysDown()
    {
        var monitor = BuildMonitor();
        monitor.RecordFailure();

        TimeSpan outage = default;
        for (var minute = 0; minute < 60; minute++)
        {
            _dateTimeProvider.AdvanceBy(TimeSpan.FromMinutes(1));
            outage = monitor.RecordFailure();
        }

        outage.Should().Be(TimeSpan.FromHours(1));
    }

    // A quiet gap means the database came back, so the next failure is a new outage starting from zero
    // rather than the old one resuming near its threshold.
    [Fact]
    public void RecordFailure_ShouldStartAgain_WhenTheDatabaseRecoveredInBetween()
    {
        var monitor = BuildMonitor();
        monitor.RecordFailure();
        _dateTimeProvider.AdvanceBy(TimeSpan.FromMinutes(30));

        monitor.RecordFailure().Should().Be(TimeSpan.Zero);
    }

    private static InvalidOperationException PoolExhaustion() =>
        new("Timeout expired. The timeout period elapsed prior to obtaining a connection from the pool. "
            + "This may have occurred because all pooled connections were in use and max pool size was reached.");
}
