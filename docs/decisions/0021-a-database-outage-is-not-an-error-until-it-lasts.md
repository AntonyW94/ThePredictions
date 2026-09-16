# 0021. A database outage is not an Error until it lasts

- **Status:** Accepted
- **Date:** 2026-09-16
- **Deciders:** Antony Willson
- **Tags:** technical

## Context

A fortnight of production logs (Sep 2-16 2026) carried **38 Errors, and 37 of them were one thing**: the database being unreachable.

```
Microsoft.Data.SqlClient.SqlException: A network-related or instance-specific error occurred
while establishing a connection to SQL Server...
 ---> System.ComponentModel.Win32Exception (64): The specified network name is no longer available.
```

They fell into five windows, four of them in the small hours, the longest about ten minutes:

| When | Duration |
|------|----------|
| Sep 3, 00:24 | seconds |
| Sep 3, 10:45 | seconds |
| Sep 9, 04:32-04:33 | ~1 minute |
| Sep 10, 04:03-04:11 | ~8 minutes |
| Sep 16, 03:57-04:06 | ~10 minutes |

**Production and development failed within 80-200ms of each other every single time.** Two separately deployed applications cannot fail in lockstep by coincidence; the only thing they share is the SQL Server instance. This is the shared hosting going away, not our code breaking.

33 of the 37 arrived through `/api/tasks/score-update`, which is not a property of that endpoint beyond it running every minute and therefore being the job most likely to be mid-flight when the server disappears. It self-heals on the next tick, and the visible consequence is that scores were stale for a few minutes overnight.

Downtime on a shared instance is part of the hosting contract. There is no ticket to raise and no change to make: nobody can act on a ten-minute outage at four in the morning, and by the time anybody read the alert it had already fixed itself. [ADR-0018](./0018-log-severity-says-who-must-act.md) settled that severity says **who has to act**, and by that rule these were never Errors - but the classification it introduced only covered client faults, so anything infrastructural still fell through to the unclassified bucket and paged.

The cost is the same one 0018 identified for warnings, one level up. `#alerts-errors` is the channel that should mean "something is broken". Five bursts a fortnight of something nobody can fix teaches people to skim it, and a real defect arrives among them.

What *would* need acting on is an outage that stops being brief. At some point the assumption that it will fix itself is no longer reasonable, and somebody has to know - to check the host, or to tell players why the site is not working.

## Decision

We will classify **database unavailability** as its own fault, and let its **duration** pick the severity.

- A failure meaning the database could not be reached is logged at **Information** while the outage has run for less than `DatabaseAvailability:OutageErrorThresholdMinutes` (**60 minutes**).
- Past that threshold the same failure is logged at **Error**.
- The status code does not move. The request did fail, so the caller still gets a 500.

`IDatabaseOutageMonitor` (Application) owns both halves, implemented by `DatabaseOutageMonitor` in the persistence adapter and registered as a **singleton**, because the outage clock has to outlive the request that noticed.

**What counts as unreachable** is a deliberately narrow set of connection-level error numbers (`-1, 40, 53, 64, 121, 233, 10053, 10054, 10060, 10061`), matched anywhere in the inner-exception chain because a read surfaces wrapped in `ReadQueryFailedException`. It is narrower than `SqlTransientFaultDetector`'s transient set, which also carries deadlocks (1205) and command timeouts (-2): those are a statement that ran and lost, they say something about our own code, and they stay Errors.

**Pool exhaustion counts only while an outage is already in progress.** `InvalidOperationException: ... prior to obtaining a connection from the pool` is what an outage looks like from inside the app once every pooled connection is stuck waiting - but it is equally what a connection leak looks like against a perfectly healthy database. Gating it on an active outage keeps a genuine leak in the Error bucket where somebody needs to see it.

**The outage clock resets on a quiet gap, not on a reported success.** A failure arriving `OutageRecoveryGraceMinutes` (**5 minutes**) or more after the previous one starts a new outage. There is no success signal to clear the clock with and there does not need to be: a database that is genuinely down produces a steady drip of failures from the per-minute score-update job, so a five-minute silence means it came back.

## Consequences

**For / positive**
- `#alerts-errors` goes back to meaning something is broken. On the fortnight reviewed, 37 of 38 Errors become Information and the channel would have fired once.
- A genuinely sustained outage still pages, which is the case that was previously indistinguishable from a ten-minute blip.
- Nothing is lost from the logs. The exception is recorded in full at Information, with the outage duration and request path, so an incident can still be reconstructed.
- The severity rule from 0018 now covers infrastructure as well as client faults, so "unclassified" once again means what it says.

**Against / cost**
- The outage clock is in-memory, so a process restart during an outage restarts it. That delays escalation rather than causing a false one, which is the safe direction, but a site recycling repeatedly through a long outage could in principle never reach the threshold. The hourly `health-check.yml` is the backstop: it fails on an unhealthy database and reports to `#github` independently of this.
- The clock depends on failures continuing to arrive. If every database-touching job were disabled the outage would not accumulate - though in that state nothing is failing either.
- Pool exhaustion is matched on message text, because the provider reports it as a bare `InvalidOperationException` with nothing else to key on. If the driver rewords it the match fails safe: the exception stays unrecognised and is reported as an Error, exactly as before.
- An outage that recurs in bursts just under five minutes apart is counted as one continuous outage and could reach Error sooner than a strict reading of "down for an hour" implies. Given what the threshold is for, escalating a flapping database is the right answer rather than a defect.

**Neutral / notes**
- 60 and 5 minutes are both configuration, so neither needs a deploy to change.
- The one Error in the fortnight that was *not* this - a single `AuthenticationFailureException` on `/signin-google` - is untouched and still reported as an Error.
- The write path has no retry policy at all: `ISqlRetryPolicy` is injected only into `DapperReadDbConnection`, so commands like the score update never retried a connection failure. That is real but separate, and it would not have helped here - every outage above outlasted any retry budget worth having.

## Alternatives considered

- **Filter the errors out in the Datadog monitor.** Rejected for the reason 0018 gives for the same idea: it puts the classification outside the repository, invisible to anyone reading the middleware, and it cannot express "unless it has been going on for an hour" without duplicating the state this class already holds.
- **Log them at Warning instead.** Moves the noise from `#alerts-errors` to `#alerts-warnings` rather than removing it, and that monitor fires on more than zero. It would have undone the same day's work making warnings readable.
- **Drop the logs entirely below the threshold.** Cheapest, and loses the evidence. The five windows above were only reconstructable because every occurrence was recorded; a future argument with the host, or a check on whether outages are getting worse, needs the same data.
- **Alert on a count of outage records instead of a duration.** A rate says how noisy an outage is, not how long it has lasted - a brief burst of many failures would page while a quiet hour of downtime would not, which is exactly backwards.
- **Have the health check own the escalation.** It already probes hourly and reports a failing database. But it only knows what is true at the moment it runs, cannot say how long the outage has been going, and reports to `#github` rather than the errors channel. Kept as the backstop rather than the mechanism.

## Related

- [ADR-0018](./0018-log-severity-says-who-must-act.md) - the severity rule this applies to infrastructure faults. Not superseded; this extends its classification to a category that previously fell through to Error.
- [ADR-0016](./0016-business-rule-exception-classification.md) - the unclassified-means-server-fault default, still the behaviour for everything not matched here.
- [`docs/todo/architecture/query-monitoring/README.md`](../todo/architecture/query-monitoring/README.md) - the same shared-instance limits seen from the slow-read side.
- [Alerting configuration](../todo/architecture/alerting-config/README.md) - the monitor definitions this keeps meaningful.
