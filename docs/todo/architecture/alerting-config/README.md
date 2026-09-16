# Alerting Configuration

## Status

Not Started | **In Progress** | Complete

Most of this shipped on 2026-07-28. Only response-time alerting remains - see Residual below.

## Summary

Configure alerting rules so problems announce themselves rather than waiting to be noticed.

## Priority

**Medium** - the alerts that matter (errors, warnings, site down) are live. What is left is
performance alerting, which needs a latency signal we do not currently collect.

## Requirements

- [x] Configure error rate alerts - Datadog log monitor on `status:error`
- [ ] Configure response time alerts - **residual**, see below
- [x] Configure availability alerts - hourly GitHub Actions health check, not Datadog
- [x] Configure database connection alerts - the health check fails on an unhealthy database
- [x] Set up notification channels - Slack

## What Shipped

### Datadog log monitors

| Monitor | Query | Grouped by | Channel |
|---------|-------|-----------|---------|
| `Web Errors [{{env.name}}]` | `status:error service:the-predictions-web` | `service`, `env`, `@error.kind` | `#alerts-errors` |
| `Web Warnings [{{env.name}}]` | `status:warn service:the-predictions-web -@Properties.SourceContext:ThePredictions.Persistence.SqlServer.Data.DapperReadDbConnection` | `service`, `env` | `#alerts-warnings` |
| `Web Slow Reads [{{env.name}}]` | `status:warn service:the-predictions-web @Properties.SourceContext:ThePredictions.Persistence.SqlServer.Data.DapperReadDbConnection` | `service`, `env` | `#alerts-warnings` |

The first two renotify every 30 minutes while unresolved, and evaluate over 5 minutes with missing
data treated as zero.

**Database outages are filed at Information until they last an hour (2026-09-16).** The errors monitor
needed the same treatment as the warnings one, for the same reason. Over Sep 2-16 2026, **37 of the 38
Errors were the shared SQL instance going away** - production and development failing within 80-200ms of
each other, in five windows lasting from seconds to ten minutes. Nobody can act on that, so it is no
longer reported as an Error unless the outage passes 60 minutes. The monitor query is unchanged: the
classification lives in the code, where anyone reading the middleware can see it. See
[ADR-0021](../../../decisions/0021-a-database-outage-is-not-an-error-until-it-lasts.md).

**Slow reads are counted, not announced one by one (2026-09-16).** Over Sep 2-16 2026, 114 of the
123 Warnings were slow-query warnings - 93% of everything the warnings monitor fired on - so a
genuine warning of any other kind was buried. `Web Warnings` now excludes them by logger rather than
by message text, which survives any rewording of the log line, and leaves that monitor for the
warnings a person can act on one at a time. `Web Slow Reads` picks them up instead and alerts only
above **25 in a rolling hour**, because one unlucky page load firing fifteen of them is not a
degradation and paging on it is what caused the fatigue. It does not renotify.

Exclude by `@Properties.SourceContext`, not by matching the message. `DapperReadDbConnection` logs
only those two warnings (`Slow query` and `Slow connection acquisition`), so the logger name is an
exact partition, and the two monitors stay complements of each other when the wording changes.
Verified at the time: the exclusion took the warnings monitor from 123 matches to 9, all of them
`Slow transaction` warnings from the scoring command.

**Group by `env`, always.** A multi-alert monitor holds a separate alert state per group, so
grouping by environment is what lets a production breach notify while dev is already alerting.
Without it the second environment is silently absorbed.

**Do not group the warnings monitor by `@error.kind`.** Slow-query warnings carry no exception,
so every one of them lands in a single `N/A` group.

### Uptime monitoring

`.github/workflows/health-check.yml` polls production hourly and reports failures to `#github`.
This exists because the Datadog monitors only fire when something **is** logged - a site that
stops serving, a broken log sink or a dead host all look identical to health, and there is no
Datadog Agent on this shared hosting to supply infrastructure metrics. See
[[../apm-integration/README.md]] for why an Agent is not an option.

Liveness uses `/health/live` (no checks, so 200 means the process is serving). Readiness parses
`/health/ready` and treats **only the database** as fatal - the football API is also covered
there, but a third party's outage is not our site being down.

### Workflow outcome notifications

`.github/workflows/notify-slack.yml` is a `workflow_call` target posting to `#github` via an
incoming webhook (`SLACK_WEBHOOK_URL` repository secret). Called from all eight top-level
workflows; deploys, migrations and the dev refresh report both outcomes, CI and the nightly
backup report failures only.

Never call it from `migrate-shared.yml` - that has seven callers and would post a duplicate
every time a deploy ran its migration step.

Slack builds notification previews from the **top-level `text` field**. A payload of only
blocks or attachments shows as "No preview available" on desktop and mobile.

**A cancelled run notifies nothing, in every workflow.** Each caller guards its `notify` job
with `if: ${{ !cancelled() }}` instead of `always()`. A cancellation is either routine
(`e2e.yml` is the one workflow with `cancel-in-progress`, so two merges a minute apart cancel
the first run) or somebody deliberately stopping a job, and in that second case they already
know. Either way there is nothing to tell them.

Note the guard has to be on the job. Dropping `cancelled` from the caller's `status` expression
is not equivalent: the six callers passing `notify-on: always` post whatever status they are
given, so a cancelled deploy would announce a **success**.

## Residual

**Response-time alerting.** There is no latency signal to alert on today. Request duration is
not recorded as a metric, only inferred from the slow-query warnings, which measure a single
read rather than a request. Options, cheapest first:

1. Generate a log-based metric in Datadog from the durations `LoggingBehaviour` already emits
   for every command, then alert on a percentile. No code change, no ingestion cost.
2. Collect proper request timings, which realistically means the APM work in
   [[../apm-integration/README.md]] - constrained by hosting, so read that first.

Also outstanding: `Datadog:Host` still carries the environment name (`development` /
`production`) rather than machine identity, predating the `env` tag. Worth repointing at a real
hostname once nothing depends on it.

## Technical Notes

The Datadog Serilog sink emits `env:local` / `env:dev` / `env:prod` from `Datadog:Env` per
`appsettings.{Environment}.json`. `env` is a Datadog reserved primary tag, so monitors and
dashboards scope by environment directly.

Service name is `the-predictions-web`, source `csharp`, on the **EU** site
(`app.datadoghq.eu`). The Web host serves the API in-process, so there is one service covering
both; `@Properties.SourceContext` distinguishes the layers.

Slack integration is installed against the `The_Predictions` workspace with `#alerts-errors`
and `#alerts-warnings` registered. With a single workspace connected the short handle form
(`@slack-alerts-errors`) resolves; the account-prefixed form is only needed with several.

`CorrelationId` is a **facet** in Datadog (created 2026-08-03), so an error log can be filtered
down to every other line from the same request. `CorrelationIdMiddleware` stamps it on every request
via `LogContext.PushProperty`, and Serilog's `FromLogContext` enricher carries it to the sink.

Note the correlation id identifies **one HTTP request**, not a user journey: the Blazor client never
sends `X-Correlation-Id`, so the middleware generates a fresh GUID per request and a page load making
five API calls produces five unrelated ids. Making the client generate and propagate one is a small
change to its HTTP handler, deliberately not done - per-request tracing answers "what happened in the
request that failed", which is what the alerting needs.
