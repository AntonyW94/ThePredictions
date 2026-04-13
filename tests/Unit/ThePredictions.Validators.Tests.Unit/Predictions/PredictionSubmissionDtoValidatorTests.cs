using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Predictions;
using ThePredictions.Validators.Predictions;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Predictions;

public class PredictionSubmissionDtoValidatorTests
{
    private readonly PredictionSubmissionDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var dto = new PredictionSubmissionDtoBuilder().Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchIdIsZero()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithMatchId(0)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchIdIsNegative()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithMatchId(-1)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeScoreIsNegative()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithHomeScore(-1)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeScoreExceeds9()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithHomeScore(10)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldPass_WhenHomeScoreIsZero()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithHomeScore(0)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldPass_WhenHomeScoreIs9()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithHomeScore(9)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayScoreIsNegative()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithAwayScore(-1)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AwayScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayScoreExceeds9()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithAwayScore(10)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AwayScore);
    }

    [Fact]
    public void Validate_ShouldPass_WhenAwayScoreIsZero()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithAwayScore(0)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.AwayScore);
    }

    [Fact]
    public void Validate_ShouldPass_WhenAwayScoreIs9()
    {
        var dto = new PredictionSubmissionDtoBuilder()
            .WithAwayScore(9)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.AwayScore);
    }
}
