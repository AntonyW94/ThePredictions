using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Boosts;
using ThePredictions.Validators.Boosts;

namespace ThePredictions.Validators.Tests.Unit.Boosts;

public class ApplyBoostRequestValidatorTests
{
    private readonly ApplyBoostRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new ApplyBoostRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenLeagueIdIsZero()
    {
        var request = new ApplyBoostRequestBuilder()
            .WithLeagueId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LeagueId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLeagueIdIsNegative()
    {
        var request = new ApplyBoostRequestBuilder()
            .WithLeagueId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LeagueId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoundIdIsZero()
    {
        var request = new ApplyBoostRequestBuilder()
            .WithRoundId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoundId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoundIdIsNegative()
    {
        var request = new ApplyBoostRequestBuilder()
            .WithRoundId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoundId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenBoostCodeIsEmpty()
    {
        var request = new ApplyBoostRequestBuilder()
            .WithBoostCode("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.BoostCode);
    }

    [Fact]
    public void Validate_ShouldFail_WhenBoostCodeExceeds50Characters()
    {
        var request = new ApplyBoostRequestBuilder()
            .WithBoostCode(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.BoostCode);
    }
}
