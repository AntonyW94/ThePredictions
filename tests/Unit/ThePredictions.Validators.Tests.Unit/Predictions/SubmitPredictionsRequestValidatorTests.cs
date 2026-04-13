using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Predictions;
using ThePredictions.Validators.Predictions;

namespace ThePredictions.Validators.Tests.Unit.Predictions;

public class SubmitPredictionsRequestValidatorTests
{
    private readonly SubmitPredictionsRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new SubmitPredictionsRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoundIdIsZero()
    {
        var request = new SubmitPredictionsRequestBuilder()
            .WithRoundId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoundId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoundIdIsNegative()
    {
        var request = new SubmitPredictionsRequestBuilder()
            .WithRoundId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoundId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPredictionsIsEmpty()
    {
        var request = new SubmitPredictionsRequestBuilder()
            .WithPredictions([])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Predictions);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPredictionsIsNull()
    {
        var request = new SubmitPredictionsRequestBuilder()
            .WithPredictions(null!)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Predictions);
    }

    [Fact]
    public void Validate_ShouldFail_WhenChildPredictionIsInvalid()
    {
        var request = new SubmitPredictionsRequestBuilder()
            .WithPredictions([new PredictionSubmissionDtoBuilder()
                .WithMatchId(0)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveAnyValidationError();
    }
}
