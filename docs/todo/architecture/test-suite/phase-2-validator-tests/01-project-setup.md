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

### Step 4: Verify Build

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

## Notes

- The `FluentValidation.TestHelper` version must be compatible with the `FluentValidation` version used by the main Validators project. Check `PredictionLeague.Validators.csproj` for the installed FluentValidation version and match accordingly.
- The `PredictionLeague.Contracts` reference is needed because the validators validate request/DTO types from that project.
