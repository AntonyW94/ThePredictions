# Task 1: Project Setup

**Parent Plan:** [Phase 2: Validator Unit Tests](./README.md)

## Status

**Not Started** | In Progress | Complete

## Goal

Create the `ThePredictions.Validators.Tests.Unit` test project with all required package references and add it to the solution.

## Files to Modify

| File | Action | Purpose |
|------|--------|---------|
| `tests/Unit/ThePredictions.Validators.Tests.Unit/ThePredictions.Validators.Tests.Unit.csproj` | Create | New test project |
| `tests/Shared/ThePredictions.Tests.Shared/ThePredictions.Tests.Shared.csproj` | Modify | Add `PredictionLeague.Contracts` reference |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAuthenticationRequests.cs` | Create | Login, Register, RefreshToken, PasswordReset builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidLeagueRequests.cs` | Create | CreateLeague, UpdateLeague, DefinePrizeStructure, JoinLeague builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidPredictionRequests.cs` | Create | SubmitPredictions, PredictionSubmissionDto, ApplyBoost builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAdminMatchRequests.cs` | Create | CreateMatch, UpdateMatch builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAdminRoundRequests.cs` | Create | CreateRound, UpdateRound, MatchResultDto builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAdminSeasonRequests.cs` | Create | CreateSeason, UpdateSeason builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAdminTeamRequests.cs` | Create | CreateTeam, UpdateTeam builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAdminUserRequests.cs` | Create | DeleteUser, UpdateUserRole builders |
| `tests/Shared/ThePredictions.Tests.Shared/Builders/ValidAccountRequests.cs` | Create | UpdateUserDetails builder |
| `PredictionLeague.sln` | Modify | Add project to solution under `Tests\Unit` folder |

## Implementation Steps

### Step 1: Create the Test Project

Create the project file with all required dependencies:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit.v3" Version="3.0.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.0.0" />
    <PackageReference Include="FluentAssertions" Version="7.0.0" />
    <PackageReference Include="FluentValidation.TestHelper" Version="11.11.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\src\PredictionLeague.Validators\PredictionLeague.Validators.csproj" />
    <ProjectReference Include="..\..\..\src\PredictionLeague.Contracts\PredictionLeague.Contracts.csproj" />
  </ItemGroup>
</Project>
```

**Key package:** `FluentValidation.TestHelper` provides `TestValidate()`, `ShouldHaveValidationErrorFor()`, and `ShouldNotHaveAnyValidationErrors()`.

**Note:** The project also references `ThePredictions.Tests.Shared` for shared request builders (see Step 4).

### Step 2: Add to Solution

```bash
dotnet sln PredictionLeague.sln add tests/Unit/ThePredictions.Validators.Tests.Unit/ThePredictions.Validators.Tests.Unit.csproj --solution-folder "Tests\Unit"
```

### Step 3: Create Folder Structure

Create the test folder structure mirroring the validators project:

```
tests/Unit/ThePredictions.Validators.Tests.Unit/
├── Account/
├── Admin/
│   ├── Matches/
│   ├── Rounds/
│   ├── Seasons/
│   ├── Teams/
│   └── Users/
├── Authentication/
├── Boosts/
├── Leagues/
└── Predictions/
```

### Step 4: Add Contracts Reference to Shared Test Project

The shared test project currently only references `PredictionLeague.Domain`. Add a reference to `PredictionLeague.Contracts` so shared request builders can create valid request objects:

```xml
<!-- Add to ThePredictions.Tests.Shared.csproj -->
<ProjectReference Include="..\..\..\src\PredictionLeague.Contracts\PredictionLeague.Contracts.csproj" />
```

### Step 5: Create Shared Request Builders

Create one static factory class per area under `tests/Shared/ThePredictions.Tests.Shared/Builders/`. Each class returns a fully valid request object that tests can then mutate for their specific scenario. This makes the builders reusable across future test projects (Application handler tests, API controller tests, etc.).

**Pattern:** Each factory method returns a valid, mutable request. Tests modify only the property under test:

```csharp
// In the test:
var request = ValidLeagueRequests.CreateLeagueRequest();
request.Name = "";  // only mutate the property being tested
var result = _validator.TestValidate(request);
```

**Builders to create (one file each):**

#### `ValidAuthenticationRequests.cs`

```csharp
public static class ValidAuthenticationRequests
{
    public static LoginRequest LoginRequest() => new()
    {
        Email = "user@example.com",
        Password = "ValidPass1"
    };

    public static RegisterRequest RegisterRequest() => new()
    {
        FirstName = "John",
        LastName = "Smith",
        Email = "john.smith@example.com",
        Password = "ValidPass1"
    };

    public static RefreshTokenRequest RefreshTokenRequest() => new()
    {
        Token = "valid-refresh-token"
    };

    public static RequestPasswordResetRequest RequestPasswordResetRequest() => new()
    {
        Email = "user@example.com"
    };

    public static ResetPasswordRequest ResetPasswordRequest() => new()
    {
        Token = "valid-reset-token",
        NewPassword = "NewValidPass1",
        ConfirmPassword = "NewValidPass1"
    };
}
```

#### `ValidLeagueRequests.cs`

```csharp
public static class ValidLeagueRequests
{
    public static CreateLeagueRequest CreateLeagueRequest() => new()
    {
        Name = "Test League",
        SeasonId = 1,
        Price = 10.00m,
        EntryDeadlineUtc = new DateTime(2099, 6, 1, 12, 0, 0, DateTimeKind.Utc),
        PointsForExactScore = 3,
        PointsForCorrectResult = 1
    };

    public static UpdateLeagueRequest UpdateLeagueRequest() => new()
    {
        Name = "Updated League",
        Price = 10.00m,
        EntryDeadlineUtc = new DateTime(2099, 6, 1, 12, 0, 0, DateTimeKind.Utc),
        PointsForExactScore = 3,
        PointsForCorrectResult = 1
    };

    public static DefinePrizeStructureRequest DefinePrizeStructureRequest() => new()
    {
        PrizeSettings = new List<DefinePrizeSettingDto>
        {
            DefinePrizeSettingDto()
        }
    };

    public static DefinePrizeSettingDto DefinePrizeSettingDto() => new()
    {
        PrizeType = /* valid enum value */,
        PrizeAmount = 50.00m,
        Rank = 1,
        Multiplier = 1.0m,
        PrizeDescription = "First place"
    };

    public static JoinLeagueRequest JoinLeagueRequest() => new()
    {
        EntryCode = "ABC123"
    };
}
```

#### `ValidPredictionRequests.cs`

```csharp
public static class ValidPredictionRequests
{
    public static SubmitPredictionsRequest SubmitPredictionsRequest() => new()
    {
        RoundId = 1,
        Predictions = new List<PredictionSubmissionDto>
        {
            PredictionSubmissionDto()
        }
    };

    public static PredictionSubmissionDto PredictionSubmissionDto() => new()
    {
        MatchId = 1,
        HomeScore = 2,
        AwayScore = 1
    };

    public static ApplyBoostRequest ApplyBoostRequest() => new()
    {
        LeagueId = 1,
        RoundId = 1,
        BoostCode = "DOUBLE"
    };
}
```

#### `ValidAdminMatchRequests.cs`

```csharp
public static class ValidAdminMatchRequests
{
    public static CreateMatchRequest CreateMatchRequest() => new()
    {
        HomeTeamId = 1,
        AwayTeamId = 2,
        MatchDateTimeUtc = new DateTime(2025, 6, 15, 15, 0, 0, DateTimeKind.Utc)
    };

    public static UpdateMatchRequest UpdateMatchRequest() => new()
    {
        HomeTeamId = 1,
        AwayTeamId = 2,
        MatchDateTimeUtc = new DateTime(2025, 6, 15, 15, 0, 0, DateTimeKind.Utc)
    };
}
```

#### `ValidAdminRoundRequests.cs`

```csharp
public static class ValidAdminRoundRequests
{
    public static CreateRoundRequest CreateRoundRequest() => new()
    {
        SeasonId = 1,
        RoundNumber = 5,
        StartDateUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        DeadlineUtc = new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc),
        Matches = new List<CreateMatchRequest>
        {
            ValidAdminMatchRequests.CreateMatchRequest()
        }
    };

    public static UpdateRoundRequest UpdateRoundRequest() => new()
    {
        StartDateUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        DeadlineUtc = new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc),
        Matches = new List<UpdateMatchRequest>
        {
            ValidAdminMatchRequests.UpdateMatchRequest()
        }
    };

    public static MatchResultDto MatchResultDto() => new()
    {
        MatchId = 1,
        HomeScore = 2,
        AwayScore = 1,
        Status = /* valid enum value */
    };
}
```

#### `ValidAdminSeasonRequests.cs`

```csharp
public static class ValidAdminSeasonRequests
{
    public static CreateSeasonRequest CreateSeasonRequest() => new()
    {
        Name = "Premier League 2025-26",
        StartDateUtc = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        EndDateUtc = new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc),
        NumberOfRounds = 38
    };

    public static UpdateSeasonRequest UpdateSeasonRequest() => new()
    {
        Name = "Premier League 2025-26",
        StartDateUtc = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        EndDateUtc = new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc),
        NumberOfRounds = 38
    };
}
```

#### `ValidAdminTeamRequests.cs`

```csharp
public static class ValidAdminTeamRequests
{
    public static CreateTeamRequest CreateTeamRequest() => new()
    {
        Name = "Manchester United",
        ShortName = "Man United",
        LogoUrl = "https://example.com/logo.png",
        Abbreviation = "MUN"
    };

    public static UpdateTeamRequest UpdateTeamRequest() => new()
    {
        Name = "Manchester United",
        ShortName = "Man United",
        LogoUrl = "https://example.com/logo.png",
        Abbreviation = "MUN"
    };
}
```

#### `ValidAdminUserRequests.cs`

```csharp
public static class ValidAdminUserRequests
{
    public static DeleteUserRequest DeleteUserRequest() => new()
    {
        NewAdministratorId = null
    };

    public static UpdateUserRoleRequest UpdateUserRoleRequest() => new()
    {
        NewRole = "Admin"  // use an actual ApplicationUserRole enum name
    };
}
```

#### `ValidAccountRequests.cs`

```csharp
public static class ValidAccountRequests
{
    public static UpdateUserDetailsRequest UpdateUserDetailsRequest() => new()
    {
        FirstName = "John",
        LastName = "Smith",
        PhoneNumber = "07123456789"
    };
}
```

### Step 6: Add Shared Project Reference to Test Project

Update the test project `.csproj` to also reference the shared project:

```xml
<ProjectReference Include="..\..\Shared\ThePredictions.Tests.Shared\ThePredictions.Tests.Shared.csproj" />
```

### Step 7: Verify Build

```bash
dotnet build PredictionLeague.sln
```

Confirm the new test project compiles with all dependencies resolved. The `FluentValidation.TestHelper` version should be compatible with the FluentValidation version used by the Validators project (12.1.1). If not, adjust the TestHelper version accordingly.

## Code Patterns to Follow

Match the existing domain test project structure:

```xml
<!-- From ThePredictions.Domain.Tests.Unit.csproj -->
<PackageReference Include="xunit.v3" Version="3.0.0" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.0.0" />
<PackageReference Include="FluentAssertions" Version="7.0.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.0" />
```

## Verification

- [ ] Project compiles without errors
- [ ] `dotnet test` runs on the new project (0 tests initially is fine)
- [ ] Project appears under `Tests\Unit` solution folder in the solution
- [ ] FluentValidation.TestHelper is compatible with the FluentValidation version in the Validators project
- [ ] All folder directories are created
- [ ] Shared project has `PredictionLeague.Contracts` reference
- [ ] `ValidMatchRequestBuilder` is accessible from the test project
- [ ] `tools\Test Coverage\coverage-unit.bat` discovers and runs the new test project (check that it finds the `.csproj` under `tests\Unit\`)

## Notes

- The `FluentValidation.TestHelper` version must be compatible with the `FluentValidation` version used by the main Validators project. Check `PredictionLeague.Validators.csproj` for the installed FluentValidation version and match accordingly.
- The `PredictionLeague.Contracts` reference is needed because the validators validate request/DTO types from that project.
- The shared project reference is needed for `ValidMatchRequestBuilder` which is used by both match and round validator tests.
- Coverlet will automatically measure the Validators assembly because the test project has a `ProjectReference` to it. No changes to `coverage.runsettings` are needed.
- No `[ExcludeFromCodeCoverage]` should be needed — all validator code is testable through the public API.
