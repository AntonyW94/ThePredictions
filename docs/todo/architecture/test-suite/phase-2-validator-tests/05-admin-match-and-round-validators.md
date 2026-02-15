# Task 5: Admin Match & Round Validator Tests

**Parent Plan:** [Phase 2: Validator Unit Tests](./README.md)

## Status

**Not Started** | In Progress | Complete

## Goal

Write unit tests for the 5 admin match and round validators, including testing inherited base match rules through both concrete implementations.

## Files to Modify

| File | Action | Purpose |
|------|--------|---------|
| `tests/Unit/ThePredictions.Validators.Tests.Unit/Admin/Matches/CreateMatchRequestValidatorTests.cs` | Create | Create match validation tests (includes base rules) |
| `tests/Unit/ThePredictions.Validators.Tests.Unit/Admin/Matches/UpdateMatchRequestValidatorTests.cs` | Create | Update match validation tests (includes base rules) |
| `tests/Unit/ThePredictions.Validators.Tests.Unit/Admin/Rounds/CreateRoundRequestValidatorTests.cs` | Create | Create round validation tests |
| `tests/Unit/ThePredictions.Validators.Tests.Unit/Admin/Rounds/UpdateRoundRequestValidatorTests.cs` | Create | Update round validation tests |
| `tests/Unit/ThePredictions.Validators.Tests.Unit/Admin/Rounds/MatchResultDtoValidatorTests.cs` | Create | Match result validation tests |

## Implementation Steps

### Step 1: CreateMatchRequestValidatorTests

**Validator:** `CreateMatchRequestValidator` (inherits `BaseMatchRequestValidator<CreateMatchRequest>`)
**Validates:** `CreateMatchRequest`

Tests all rules from `BaseMatchRequestValidator` through the concrete `CreateMatchRequestValidator`:

| Test | Property | Scenario |
|------|----------|----------|
| `Validate_ShouldPass_WhenAllFieldsAreValid` | All | Valid home/away teams and match date |
| `Validate_ShouldFail_WhenHomeTeamIdIsZero` | HomeTeamId | Value 0 |
| `Validate_ShouldFail_WhenHomeTeamIdIsNegative` | HomeTeamId | Negative number |
| `Validate_ShouldFail_WhenAwayTeamIdIsZero` | AwayTeamId | Value 0 |
| `Validate_ShouldFail_WhenAwayTeamIdIsNegative` | AwayTeamId | Negative number |
| `Validate_ShouldFail_WhenHomeAndAwayTeamsAreTheSame` | AwayTeamId | Same value as HomeTeamId |
| `Validate_ShouldFail_WhenMatchDateTimeUtcIsDefault` | MatchDateTimeUtc | `default(DateTime)` |

### Step 2: UpdateMatchRequestValidatorTests

**Validator:** `UpdateMatchRequestValidator` (inherits `BaseMatchRequestValidator<UpdateMatchRequest>`)
**Validates:** `UpdateMatchRequest`

Tests the same base rules but through `UpdateMatchRequestValidator` to confirm inheritance works:

| Test | Property | Scenario |
|------|----------|----------|
| `Validate_ShouldPass_WhenAllFieldsAreValid` | All | Valid update request |
| `Validate_ShouldFail_WhenHomeTeamIdIsZero` | HomeTeamId | Value 0 |
| `Validate_ShouldFail_WhenAwayTeamIdIsZero` | AwayTeamId | Value 0 |
| `Validate_ShouldFail_WhenHomeAndAwayTeamsAreTheSame` | AwayTeamId | Same value as HomeTeamId |
| `Validate_ShouldFail_WhenMatchDateTimeUtcIsDefault` | MatchDateTimeUtc | `default(DateTime)` |

### Step 3: CreateRoundRequestValidatorTests

**Validator:** `CreateRoundRequestValidator`
**Validates:** `CreateRoundRequest`

| Test | Property | Scenario |
|------|----------|----------|
| `Validate_ShouldPass_WhenAllFieldsAreValid` | All | Valid round with matches |
| `Validate_ShouldFail_WhenSeasonIdIsZero` | SeasonId | Value 0 |
| `Validate_ShouldFail_WhenSeasonIdIsNegative` | SeasonId | Negative number |
| `Validate_ShouldFail_WhenRoundNumberIsZero` | RoundNumber | Value 0 (min is 1) |
| `Validate_ShouldFail_WhenRoundNumberExceeds52` | RoundNumber | Value 53 (max is 52) |
| `Validate_ShouldPass_WhenRoundNumberIs1` | RoundNumber | Value 1 (lower boundary) |
| `Validate_ShouldPass_WhenRoundNumberIs52` | RoundNumber | Value 52 (upper boundary) |
| `Validate_ShouldFail_WhenStartDateUtcIsDefault` | StartDateUtc | `default(DateTime)` |
| `Validate_ShouldFail_WhenDeadlineUtcIsDefault` | DeadlineUtc | `default(DateTime)` |
| `Validate_ShouldFail_WhenDeadlineIsBeforeStartDate` | DeadlineUtc | Earlier than StartDateUtc |
| `Validate_ShouldFail_WhenMatchesIsEmpty` | Matches | Empty collection |
| `Validate_ShouldFail_WhenMatchesIsNull` | Matches | Null collection |
| `Validate_ShouldFail_WhenChildMatchIsInvalid` | Matches[] | Match with HomeTeamId = 0 (nested validator) |

```csharp
public class CreateRoundRequestValidatorTests
{
    private readonly CreateRoundRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new CreateRoundRequest
        {
            SeasonId = 1,
            RoundNumber = 5,
            StartDateUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            DeadlineUtc = new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc),
            Matches = new List<CreateMatchRequest>
            {
                new()
                {
                    HomeTeamId = 1,
                    AwayTeamId = 2,
                    MatchDateTimeUtc = new DateTime(2025, 1, 3, 15, 0, 0, DateTimeKind.Utc)
                }
            }
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
```

### Step 4: UpdateRoundRequestValidatorTests

**Validator:** `UpdateRoundRequestValidator`
**Validates:** `UpdateRoundRequest`

| Test | Property | Scenario |
|------|----------|----------|
| `Validate_ShouldPass_WhenAllFieldsAreValid` | All | Valid update with matches |
| `Validate_ShouldFail_WhenStartDateUtcIsDefault` | StartDateUtc | `default(DateTime)` |
| `Validate_ShouldFail_WhenDeadlineUtcIsDefault` | DeadlineUtc | `default(DateTime)` |
| `Validate_ShouldFail_WhenDeadlineIsBeforeStartDate` | DeadlineUtc | Earlier than StartDateUtc |
| `Validate_ShouldFail_WhenMatchesIsEmpty` | Matches | Empty collection |
| `Validate_ShouldFail_WhenMatchesIsNull` | Matches | Null collection |
| `Validate_ShouldFail_WhenChildMatchIsInvalid` | Matches[] | Match with HomeTeamId = 0 |
| `Validate_ShouldPass_WhenDeadlineEqualsStartDate` | DeadlineUtc | This should fail (GreaterThan, not GreaterThanOrEqual) |

**Note:** `UpdateRoundRequest` does not have `SeasonId` or `RoundNumber` — those are immutable after creation.

### Step 5: MatchResultDtoValidatorTests

**Validator:** `MatchResultDtoValidator`
**Validates:** `MatchResultDto`

| Test | Property | Scenario |
|------|----------|----------|
| `Validate_ShouldPass_WhenAllFieldsAreValid` | All | Valid result |
| `Validate_ShouldFail_WhenMatchIdIsZero` | MatchId | Value 0 |
| `Validate_ShouldFail_WhenMatchIdIsNegative` | MatchId | Negative number |
| `Validate_ShouldFail_WhenHomeScoreIsNegative` | HomeScore | Value -1 |
| `Validate_ShouldFail_WhenHomeScoreExceedsNine` | HomeScore | Value 10 |
| `Validate_ShouldFail_WhenAwayScoreIsNegative` | AwayScore | Value -1 |
| `Validate_ShouldFail_WhenAwayScoreExceedsNine` | AwayScore | Value 10 |
| `Validate_ShouldPass_WhenScoresAreZero` | HomeScore/AwayScore | Both 0 (boundary) |
| `Validate_ShouldPass_WhenScoresAreNine` | HomeScore/AwayScore | Both 9 (boundary) |
| `Validate_ShouldFail_WhenStatusIsInvalidEnum` | Status | Invalid enum value |

## Code Patterns to Follow

### Shared Match Request Builders

Use the shared `ValidMatchRequestBuilder` from `ThePredictions.Tests.Shared` (created in Task 1) for both match and round tests. This avoids duplicating valid match request construction:

```csharp
// Match validator tests — use shared builder, then modify the field under test
public class CreateMatchRequestValidatorTests
{
    private readonly CreateMatchRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = ValidMatchRequestBuilder.CreateValidCreateMatchRequest();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeAndAwayTeamsAreTheSame()
    {
        var request = ValidMatchRequestBuilder.CreateValidCreateMatchRequest();
        request.HomeTeamId = 5;
        request.AwayTeamId = 5;

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }
}

// Round validator tests — use the same shared builder for child matches
public class CreateRoundRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new CreateRoundRequest
        {
            SeasonId = 1,
            RoundNumber = 5,
            StartDateUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            DeadlineUtc = new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc),
            Matches = new List<CreateMatchRequest>
            {
                ValidMatchRequestBuilder.CreateValidCreateMatchRequest()
            }
        };
        // ...
    }
}
```

## Verification

- [ ] All tests pass
- [ ] Base match rules tested through both `CreateMatchRequestValidator` and `UpdateMatchRequestValidator`
- [ ] "Home and away teams cannot be the same" rule tested
- [ ] "Deadline must be after start date" rule tested for both round validators
- [ ] Score boundaries (0 and 9) tested for `MatchResultDtoValidator`
- [ ] Nested match validator delegation tested for both round validators
- [ ] Enum validation tested for `MatchResultDto.Status`
- [ ] `dotnet build` succeeds with no warnings

## Edge Cases to Consider

- `MatchDateTimeUtc` as `default(DateTime)` (0001-01-01) should trigger NotEmpty failure
- `DeadlineUtc` equal to `StartDateUtc` should fail (rule is GreaterThan, not GreaterThanOrEqual)
- Home team = away team with ID 0 produces two errors (GreaterThan and NotEqual)
- Round number boundaries: 1 and 52 should pass, 0 and 53 should fail
- `CreateRoundRequest.Matches` uses `SetValidator` for `CreateMatchRequest`, while `UpdateRoundRequest.Matches` uses `SetValidator` for `UpdateMatchRequest`

## Notes

- The base validator pattern means `BaseMatchRequestValidator` has no direct test file — its rules are verified through `CreateMatchRequestValidatorTests` and `UpdateMatchRequestValidatorTests`.
- Admin validators protect data integrity for the match/round management system.
- `MatchResultDtoValidator` is used when submitting match results (actual scores) after a round is played.
