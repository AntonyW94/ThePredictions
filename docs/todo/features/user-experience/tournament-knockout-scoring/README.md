# 90-Minute Scoring for Tournament Knockout Rounds

## Status

Not Started | In Progress | **Complete**

## Summary

In tournament knockout rounds, matches can go to extra time and penalties. Predictions should be scored based on the 90-minute (regular time) result only — extra time and penalty goals should not count towards prediction outcomes.

Currently, the API sync uses the `goals` field from API-Football, which includes extra time goals. This means a match that finishes 1-1 after 90 minutes but 2-1 after extra time would incorrectly score predictions against the 2-1 result instead of the 1-1 result.

## Priority

**High** — Required before the first knockout round of any tournament (World Cup 2026 Round of 32, expected July 2026).

## Current Behaviour

The `FixtureResponse` DTO captures only the `goals` object from the API:

```csharp
// FixtureResponse.cs
[JsonPropertyName("goals")]
public Goals? Goals { get; set; }
```

The `goals` field in the API-Football response is the **final** score including extra time. The sync handler (`SyncSeasonWithApiCommandHandler`) uses this to call `Match.UpdateScore()`, which sets `ActualHomeTeamScore` and `ActualAwayTeamScore`.

`UserPrediction.DetermineOutcome()` then compares predictions against these scores.

## Desired Behaviour

For tournament knockout matches, use the **full-time (90-minute)** score for prediction scoring. The API-Football response provides separate score breakdowns:

```json
{
  "score": {
    "halftime": { "home": 0, "away": 0 },
    "fulltime": { "home": 1, "away": 1 },
    "extratime": { "home": 1, "away": 0 },
    "penalty": { "home": null, "away": null }
  },
  "goals": { "home": 2, "away": 1 }
}
```

For the example above, predictions should be scored against **1-1** (fulltime), not 2-1 (goals).

## Requirements

- [x] Add `Score` DTO to capture `fulltime`, `extratime`, and `penalty` from API response
- [x] Update `FixtureResponse` to include the `score` object
- [x] Update sync handler to use `score.fulltime` instead of `goals` for tournament knockout matches
- [x] Group stage matches are unaffected (no extra time in group stages)
- [x] League matches are unaffected (no extra time in leagues)
- [x] Unit tests for score selection logic
- [ ] Verify with API-Football 2022 World Cup data that `score.fulltime` returns the expected 90-minute scores

## Technical Notes

### Identifying Knockout Matches

Tournament knockout matches can be identified by their `TournamentStage`:
- `RoundOf32`, `RoundOf16`, `QuarterFinals`, `SemiFinals`, `ThirdPlace`, `Final` — use `score.fulltime`
- `Group1`, `Group2`, `Group3` — use `goals` (no extra time possible)

The `Match.ApiRoundName` and the `TournamentRoundMapping` stages can be used to determine which scoring source to use.

### Fallback

If `score.fulltime` is null (e.g., match not yet at full time), fall back to `goals` as currently done. This ensures live score updates still work during the first 90 minutes.

### Third Place Playoff

The third place playoff also goes to extra time/penalties if drawn. Use `score.fulltime` for this match as well.

## Dependencies

- [x] Tournament support (complete)
- [x] `TournamentStage` enumeration (complete)
- [x] `TournamentRoundNameParser` (complete)
- [x] `Match.ApiRoundName` property (complete)
