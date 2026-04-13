using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Matches;
using ThePredictions.Tests.Builders.Admin.Rounds;
using ThePredictions.Validators.Admin.Rounds;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Admin.Rounds;

public class CreateRoundRequestValidatorTests
{
    private readonly CreateRoundRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new CreateRoundRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenSeasonIdIsZero()
    {
        var request = new CreateRoundRequestBuilder()
            .WithSeasonId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeasonId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenSeasonIdIsNegative()
    {
        var request = new CreateRoundRequestBuilder()
            .WithSeasonId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeasonId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoundNumberIsZero()
    {
        var request = new CreateRoundRequestBuilder()
            .WithRoundNumber(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoundNumber);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoundNumberExceeds52()
    {
        var request = new CreateRoundRequestBuilder()
            .WithRoundNumber(53)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoundNumber);
    }

    [Fact]
    public void Validate_ShouldPass_WhenRoundNumberIs1()
    {
        var request = new CreateRoundRequestBuilder()
            .WithRoundNumber(1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.RoundNumber);
    }

    [Fact]
    public void Validate_ShouldPass_WhenRoundNumberIs52()
    {
        var request = new CreateRoundRequestBuilder()
            .WithRoundNumber(52)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.RoundNumber);
    }

    [Fact]
    public void Validate_ShouldFail_WhenStartDateUtcIsDefault()
    {
        var request = new CreateRoundRequestBuilder()
            .WithStartDateUtc(default)
            .WithDeadlineUtc(new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.StartDateUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDeadlineUtcIsDefault()
    {
        var request = new CreateRoundRequestBuilder()
            .WithDeadlineUtc(default)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.DeadlineUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDeadlineIsBeforeStartDate()
    {
        var request = new CreateRoundRequestBuilder()
            .WithStartDateUtc(new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc))
            .WithDeadlineUtc(new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.DeadlineUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchesIsEmpty()
    {
        var request = new CreateRoundRequestBuilder()
            .WithMatches([])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Matches);
    }

    [Fact]
    public void Validate_ShouldFail_WhenChildMatchIsInvalid()
    {
        var request = new CreateRoundRequestBuilder()
            .WithMatches([new CreateMatchRequestBuilder()
                .WithHomeTeamId(0)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveAnyValidationError();
    }
}
