# Task 4: Tests

**Parent Feature:** [Per-Round Historical Snapshots](./README.md)

## Status

**Not Started** | In Progress | Complete

## Goal

Catch the regressions that matter most for snapshot data: idempotency, tournament-vs-league branching, and "the snapshot at the latest round equals live state". Pure-Domain coverage stays at 100% as required by the project rules.

## Domain unit tests

In `tests/Unit/ThePredictions.Domain.Tests.Unit/Models/`:

- `LeagueMemberRoundStatsTests`
  - `Create_ShouldThrow_WhenLeagueIdNonPositive`
  - `Create_ShouldThrow_WhenUserIdNullOrWhitespace`
  - `Create_ShouldThrow_WhenRoundIdNonPositive`
  - `Create_ShouldThrow_WhenAnyCountNegative`
  - `Create_ShouldSucceed_WhenValid`
  - `Create_ShouldAcceptNullMonthlyFields_WhenTournamentRound`
  - `Create_ShouldAcceptNullBoostFields_WhenNoBoostUsed`
- `LeagueRoundSnapshotTests` - equivalents.

Domain models are simple value-style records, so most of these guard checks are trivial. They're cheap insurance.

## Application / Infrastructure integration tests

In `tests/Unit/ThePredictions.Application.Tests.Unit/` (or wherever existing repository integration tests live):

### Write path

- `WriteRoundSnapshots_ShouldInsertOneRowPerMember_WhenInvoked`
- `WriteRoundSnapshots_ShouldInsertOneSnapshotPerLeague_WhenInvoked`
- `WriteRoundSnapshots_ShouldBeIdempotent_WhenCalledTwice`
- `WriteRoundSnapshots_ShouldNullMonthlyFields_WhenTournamentSeason`
- `WriteRoundSnapshots_ShouldPopulateTournamentStage_WhenTournamentRound`
- `WriteRoundSnapshots_ShouldLeaveTournamentStageNull_WhenLeagueRound`
- `WriteRoundSnapshots_ShouldComputeCumulativePointsCorrectly_WhenMidSeason`
- `WriteRoundSnapshots_ShouldComputeRoundOutcomeCounts_FromPredictionsAndResults`
- `WriteRoundSnapshots_ShouldExcludeMidSeasonJoiners_FromEarlierRounds`

### Service wiring

- `UpdateStableStatsAsync_ShouldCallWriteRoundSnapshotsAsync_AfterStableUpdate`
- `UpdateStableStatsAsync_ShouldNotCallWriteRoundSnapshotsAsync_WhenStableUpdateFails`

### Backfill

- `Backfill_ShouldProcessRoundsInChronologicalOrder`
- `Backfill_ShouldExcludeUsersWhoJoinedAfterRound`
- `Backfill_ShouldFailValidation_WhenLatestRoundMismatchesLive`
- `Backfill_ShouldHandleEmptyState_GracefullyWhenNoRoundsCompleted`

## Test data fixtures

A small builder (`TestSeasonBuilder`?) that sets up a league with N members, N rounds, predictions, results, and known points. Existing test infrastructure in this project likely has something similar - reuse rather than rebuild.

## Coverage targets

- **Domain:** 100% line / 100% branch (project rule).
- **Application/Infrastructure:** Maintain whatever the current targets are. The new code should be covered.

## Verification

- [ ] All test classes pass.
- [ ] `tools\Test Coverage\coverage-unit.bat` reports 100% line and 100% branch on Domain.
- [ ] No `[ExcludeFromCodeCoverage]` annotations needed beyond the standard "ORM-only constructor" usage.
- [ ] CI green.
