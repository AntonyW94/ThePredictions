using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Matches;
using ThePredictions.Validators.Admin.Matches;

namespace ThePredictions.Validators.Tests.Unit.Admin.Matches;

public class CreateMatchRequestValidatorTests
{
    private readonly CreateMatchRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new CreateMatchRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeTeamIdIsZero()
    {
        var request = new CreateMatchRequestBuilder()
            .WithHomeTeamId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.HomeTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeTeamIdIsNegative()
    {
        var request = new CreateMatchRequestBuilder()
            .WithHomeTeamId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.HomeTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayTeamIdIsZero()
    {
        var request = new CreateMatchRequestBuilder()
            .WithAwayTeamId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayTeamIdIsNegative()
    {
        var request = new CreateMatchRequestBuilder()
            .WithAwayTeamId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeAndAwayTeamsAreTheSame()
    {
        var request = new CreateMatchRequestBuilder()
            .WithHomeTeamId(1)
            .WithAwayTeamId(1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchDateTimeUtcIsDefault()
    {
        var request = new CreateMatchRequestBuilder()
            .WithMatchDateTimeUtc(default)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MatchDateTimeUtc);
    }
}
