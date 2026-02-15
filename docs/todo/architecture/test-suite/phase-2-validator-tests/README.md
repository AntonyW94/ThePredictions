# Phase 2: Validator Unit Tests

**Parent Plan:** [Test Suite Plan](../README.md)

## Status

**Not Started** | In Progress | Complete

## Summary

Create unit tests for all 27 validators in the `PredictionLeague.Validators` project. Validators use FluentValidation and can be tested with `TestValidate()` making them quick wins with high confidence. This phase covers all validation rules across authentication, leagues, predictions, boosts, admin operations, and account management.

## Scope

| Category | Validators | Estimated Tests |
|----------|-----------|-----------------|
| Authentication | 5 (Login, Register, RefreshToken, RequestPasswordReset, ResetPassword) | ~31 |
| Leagues | 4 (CreateLeague, UpdateLeague, DefinePrizeStructure + nested, JoinLeague) | ~40 |
| Predictions & Boosts | 3 (SubmitPredictions, PredictionSubmissionDto, ApplyBoost) | ~17 |
| Admin Matches & Rounds | 5 (BaseMatch via Create/Update, CreateRound, UpdateRound, MatchResultDto) | ~30 |
| Admin Seasons & Teams | 4 (BaseSeason via Create/Update, BaseTeam via Create/Update) | ~24 |
| Admin Users & Account | 3 (DeleteUser, UpdateUserRole, UpdateUserDetails) | ~18 |
| **Total** | **24 concrete + 3 base** | **~160** |

## Test Approach

All validators are tested using FluentValidation's built-in `TestValidate()` method:

```csharp
public class CreateLeagueRequestValidatorTests
{
    private readonly CreateLeagueRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = new CreateLeagueRequest { Name = "My League", SeasonId = 1, ... };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateLeagueRequest { Name = "" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
```

### Base Validator Testing Strategy

Three validators use inheritance (`BaseMatchRequestValidator`, `BaseSeasonRequestValidator`, `BaseTeamRequestValidator`). The base rules are tested through both concrete implementations:

- `CreateMatchRequestValidator` tests prove the base match rules work for creation
- `UpdateMatchRequestValidator` tests prove the same rules work for updates
- Each concrete test class has a full happy-path test and all individual rule failure tests

### Common Extension Testing Strategy

The custom validation extensions (`MustBeASafeName`, `MustBeASafeLeagueName`) are tested indirectly through the validators that use them. Each validator test class includes specific test cases for the character safety rules (XSS characters rejected, valid characters accepted).

## Tasks

| # | Task | Description | Status |
|---|------|-------------|--------|
| 1 | [Project Setup](./01-project-setup.md) | Create test project, add references, configure solution | Not Started |
| 2 | [Authentication Validators](./02-authentication-validators.md) | Tests for Login, Register, RefreshToken, RequestPasswordReset, ResetPassword | Not Started |
| 3 | [League Validators](./03-league-validators.md) | Tests for CreateLeague, UpdateLeague, DefinePrizeStructure, JoinLeague | Not Started |
| 4 | [Prediction & Boost Validators](./04-prediction-and-boost-validators.md) | Tests for SubmitPredictions, PredictionSubmissionDto, ApplyBoost | Not Started |
| 5 | [Admin Match & Round Validators](./05-admin-match-and-round-validators.md) | Tests for Create/UpdateMatch (base rules), CreateRound, UpdateRound, MatchResultDto | Not Started |
| 6 | [Admin Season & Team Validators](./06-admin-season-and-team-validators.md) | Tests for Create/UpdateSeason (base rules), Create/UpdateTeam (base rules) | Not Started |
| 7 | [Admin User & Account Validators](./07-admin-user-and-account-validators.md) | Tests for DeleteUser, UpdateUserRole, UpdateUserDetails | Not Started |

## Dependencies

- [x] Phase 1: Domain Unit Tests (complete - 462 tests)
- [x] `PredictionLeague.Validators` project exists with all 27 validators
- [x] `PredictionLeague.Contracts` project exists with all request/DTO classes
- [ ] FluentValidation.TestHelper package (added in Task 1)

## Coverage

### How Coverage Works

The `tools/Test Coverage/coverage-unit.bat` script discovers and runs **all** `.csproj` files under `tests/Unit/`. The `coverage.runsettings` file has no assembly inclusion/exclusion filters — coverlet measures whatever assemblies each test project references. Adding `ThePredictions.Validators.Tests.Unit` with a `ProjectReference` to `PredictionLeague.Validators` will automatically include the Validators assembly in coverage reports.

### Coverage Target

The Validators project must achieve **100% line and branch coverage**, matching the Domain project standard. This is achievable because:

- All concrete validators have parameterised constructors only (no parameterless ORM constructors)
- All base validators have protected constructors only
- Extension methods are pure static (no untestable code)
- `[GeneratedRegex]` source-generated code is already excluded by the `[CompilerGeneratedAttribute]` filter in `coverage.runsettings`
- Private helper methods (e.g. `BeAValidUrl`) are fully reachable through the public validator API
- No `[ExcludeFromCodeCoverage]` should be needed anywhere in the Validators project

### Verifying Coverage

After completing all tasks, run:

```bash
tools\Test Coverage\coverage-unit.bat
```

The HTML report at `coverage/report/index.html` should show 100% line and 100% branch coverage for both the Domain and Validators assemblies.

## Shared Test Helpers

### Candidates for `ThePredictions.Tests.Shared`

Some request builder methods are needed across multiple test files and should live in the shared test project rather than being duplicated:

| Builder | Used By | Why Shared |
|---------|---------|------------|
| `ValidCreateMatchRequest()` | `CreateMatchRequestValidatorTests`, `CreateRoundRequestValidatorTests` | Round tests need valid child matches |
| `ValidUpdateMatchRequest()` | `UpdateMatchRequestValidatorTests`, `UpdateRoundRequestValidatorTests` | Round tests need valid child matches |

The shared project will need a new `ProjectReference` to `PredictionLeague.Contracts` to access the request types.

Request builders that are only used by a single test class (e.g. `CreateValidRequest()` for `CreateLeagueRequest`) should stay as private helpers in their test class — no need to share them.

## Technical Notes

### Package Requirements

The test project needs `FluentValidation.TestHelper` for the `TestValidate()` extension method and assertion helpers like `ShouldHaveValidationErrorFor()` and `ShouldNotHaveAnyValidationErrors()`.

### Test File Structure

```
tests/Unit/ThePredictions.Validators.Tests.Unit/
├── Account/
│   └── UpdateUserDetailsRequestValidatorTests.cs
├── Admin/
│   ├── Matches/
│   │   ├── CreateMatchRequestValidatorTests.cs
│   │   └── UpdateMatchRequestValidatorTests.cs
│   ├── Rounds/
│   │   ├── CreateRoundRequestValidatorTests.cs
│   │   ├── UpdateRoundRequestValidatorTests.cs
│   │   └── MatchResultDtoValidatorTests.cs
│   ├── Seasons/
│   │   ├── CreateSeasonRequestValidatorTests.cs
│   │   └── UpdateSeasonRequestValidatorTests.cs
│   ├── Teams/
│   │   ├── CreateTeamRequestValidatorTests.cs
│   │   └── UpdateTeamRequestValidatorTests.cs
│   └── Users/
│       ├── DeleteUserRequestValidatorTests.cs
│       └── UpdateUserRoleRequestValidatorTests.cs
├── Authentication/
│   ├── LoginRequestValidatorTests.cs
│   ├── RefreshTokenRequestValidatorTests.cs
│   ├── RegisterRequestValidatorTests.cs
│   ├── RequestPasswordResetRequestValidatorTests.cs
│   └── ResetPasswordRequestValidatorTests.cs
├── Boosts/
│   └── ApplyBoostRequestValidatorTests.cs
├── Leagues/
│   ├── CreateLeagueRequestValidatorTests.cs
│   ├── UpdateLeagueRequestValidatorTests.cs
│   ├── DefinePrizeStructureRequestValidatorTests.cs
│   └── JoinLeagueRequestValidatorTests.cs
└── Predictions/
    ├── SubmitPredictionsRequestValidatorTests.cs
    └── PredictionSubmissionDtoValidatorTests.cs
```

### Naming Convention

All tests follow: `Validate_Should{Behaviour}_When{Conditions}()`

Examples:
- `Validate_ShouldPass_WhenAllFieldsAreValid()`
- `Validate_ShouldFail_WhenNameIsEmpty()`
- `Validate_ShouldFail_WhenNameContainsHtmlTags()`
- `Validate_ShouldFail_WhenPriceIsNegative()`
