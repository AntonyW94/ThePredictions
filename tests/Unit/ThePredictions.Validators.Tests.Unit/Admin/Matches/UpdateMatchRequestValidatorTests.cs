using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Matches;
using ThePredictions.Validators.Admin.Matches;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Admin.Matches;

public class UpdateMatchRequestValidatorTests
{
    private readonly UpdateMatchRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new UpdateMatchRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeTeamIdIsZero()
    {
        var request = new UpdateMatchRequestBuilder()
            .WithHomeTeamId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.HomeTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeTeamIdIsNegative()
    {
        var request = new UpdateMatchRequestBuilder()
            .WithHomeTeamId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.HomeTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayTeamIdIsZero()
    {
        var request = new UpdateMatchRequestBuilder()
            .WithAwayTeamId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayTeamIdIsNegative()
    {
        var request = new UpdateMatchRequestBuilder()
            .WithAwayTeamId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeAndAwayTeamsAreTheSame()
    {
        var request = new UpdateMatchRequestBuilder()
            .WithHomeTeamId(1)
            .WithAwayTeamId(1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AwayTeamId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchDateTimeUtcIsDefault()
    {
        var request = new UpdateMatchRequestBuilder()
            .WithMatchDateTimeUtc(default)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MatchDateTimeUtc);
    }
}
