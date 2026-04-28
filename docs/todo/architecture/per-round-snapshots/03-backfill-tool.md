# Task 3: Backfill Tool

**Parent Feature:** [Per-Round Historical Snapshots](./README.md)

## Status

**Not Started** | In Progress | Complete

## Goal

A one-shot CLI tool that walks every completed round in chronological order across both existing leagues and writes the corresponding snapshot rows. Run it once against the dev DB, verify, then run against prod.

## Where it lives

`tools/ThePredictions.DatabaseTools/` - same project as the existing `DatabaseRefresher` and `ProductionBackup` tools. Adds a new mode:

```bash
dotnet run --project tools/ThePredictions.DatabaseTools -- BackfillRoundSnapshots
```

The tool takes optional flags:
- `--season-id <id>` - restrict to one season
- `--dry-run` - compute and print summary, don't write
- `--force` - delete existing snapshot rows first (use with care)

## Algorithm

The right approach is to **reuse the production write path**, not reinvent it.

```
1. Connect to target DB.
2. SELECT all completed rounds, ordered by Season.StartDateUtc, then RoundNumber.
3. For each round:
   a. If --dry-run: log what would be inserted.
   b. Otherwise: call WriteRoundSnapshotsAsync(roundId).
   c. Log progress.
4. Validate (see below).
```

This means the backfill is essentially: "pretend each historical round just closed, in order." The same SQL that runs in production at round close runs here.

**Important:** The backfill must populate cumulative points correctly *as of that round*. Since the production write path computes cumulative points by SUM-ing `LeagueRoundResults` up to and including the round, it works for any starting point - no need for the tool to track cumulative state in memory.

For ranks, the production write path reads from `LeagueMemberStats`. **For backfill, this is wrong** - `LeagueMemberStats` reflects current state, not the state at the historical round. Two options:

### Option 1 (recommended): recompute ranks per round during backfill

The tool, before calling `WriteRoundSnapshotsAsync` for round N, runs a temporary computation that gives the cumulative rank at round N's close. This can be a CTE:

```sql
WITH PointsUpTo AS (
    SELECT lrr.[LeagueId], lrr.[UserId],
           SUM(lrr.[BoostedPoints]) AS Cumulative
    FROM [LeagueRoundResults] lrr
    JOIN [Rounds] r ON lrr.[RoundId] = r.[Id]
    WHERE r.[StartDateUtc] <= @AsOfRoundStart
    GROUP BY lrr.[LeagueId], lrr.[UserId]
),
Ranked AS (
    SELECT [LeagueId], [UserId], [Cumulative],
           RANK() OVER (PARTITION BY [LeagueId] ORDER BY [Cumulative] DESC) AS Rk
    FROM PointsUpTo
)
SELECT * FROM Ranked;
```

Result feeds the snapshot insert.

This is essentially a backfill-only variant of the production write path. Worth abstracting into a shared method `WriteRoundSnapshotsAsync(roundId, useHistoricalRanks: bool)`.

### Option 2: use current ranks as proxy

Faster but wrong - cumulative rank at round 5 is not "current rank". Reject.

## Validation

After backfill, run sanity checks:

1. **Row counts.** Expected: one `LeagueRoundSnapshot` row per (league, completed round). One `LeagueMemberRoundStats` row per (league, member, completed round) where membership existed at that time.
2. **Most recent round consistency.** For the most recent round, snapshot values should match the live `LeagueMemberStats` (same overall rank, points). If they don't, the production write path and the backfill differ - bug.
3. **Cumulative monotonicity.** For any (user, league), `OverallPointsAfterRound` should be non-decreasing across `RoundId` ordered by round date. (Points can never go down.)
4. **Rank stability check.** Spot-check a known result: e.g. if user X is currently 3rd in their league, their `OverallRankAfterRound` for the latest round in the snapshot should also be 3.

Print a summary of all four checks at the end. Fail loudly on mismatch.

## Mid-season membership

A user who joins a league mid-season should not have rows for rounds before they joined. The membership check uses `LeagueMembers.JoinedAtUtc` (or whatever the existing column is - confirm during implementation):

```sql
WHERE lm.[JoinedAtUtc] < r.[StartDateUtc]
```

This excludes them from earlier rounds' snapshots.

## Run order

1. **Dev:** run with `--dry-run` first. Eye the output.
2. **Dev:** run for real. Validate.
3. **Refresh dev DB from prod** (you have a tool for this) to reset.
4. **Prod:** run for real. Schedule a low-traffic window.

The new live writes from Task 2 will start populating snapshot rows from the next round onwards - those don't need backfilling.

## Tests

- `Backfill_ShouldProcessRoundsInChronologicalOrder` - single league, several rounds.
- `Backfill_ShouldExcludeMidSeasonJoiners` - user joined at round 5, no rows for rounds 1-4.
- `Backfill_ShouldNullMonthlyForTournament` - tournament season, all member rows have NULL month fields.
- `Backfill_ShouldBeIdempotent` - running twice produces no duplicates.
- `Backfill_ShouldFailValidation_WhenLatestRoundDoesntMatchLive` - sanity check fires.

These probably want a small in-process test database or fixture to set up known historical state.

## Verification

- [ ] `--dry-run` against dev produces a sensible summary with no inserts.
- [ ] Real run against dev populates the expected number of rows in both tables.
- [ ] Validation checks all pass.
- [ ] Spot-checked user's latest rank in the snapshot matches their current rank.
- [ ] Tournament rounds in dev (if any exist yet) have NULL monthly fields and populated `TournamentStage`.
- [ ] Tool runs in under 60 seconds for the existing dataset (34 rounds, 2 leagues).
- [ ] Tool documented in `tools/ThePredictions.DatabaseTools/README.md` (or wherever the existing tools are documented).
