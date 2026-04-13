using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Authentication;
using ThePredictions.Validators.Authentication;

namespace ThePredictions.Validators.Tests.Unit.Authentication;

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new ResetPasswordRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTokenIsEmpty()
    {
        var request = new ResetPasswordRequestBuilder()
            .WithToken("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewPasswordIsEmpty()
    {
        var request = new ResetPasswordRequestBuilder()
            .WithNewPassword("")
            .WithConfirmPassword("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewPasswordIsTooShort()
    {
        var request = new ResetPasswordRequestBuilder()
            .WithNewPassword("Short1")
            .WithConfirmPassword("Short1")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Validate_ShouldFail_WhenConfirmPasswordIsEmpty()
    {
        var request = new ResetPasswordRequestBuilder()
            .WithConfirmPassword("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void Validate_ShouldFail_WhenConfirmPasswordDoesNotMatch()
    {
        var request = new ResetPasswordRequestBuilder()
            .WithNewPassword("NewValidPass1")
            .WithConfirmPassword("DifferentPass1")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPasswordsMatch()
    {
        var request = new ResetPasswordRequestBuilder()
            .WithNewPassword("MatchingPass1")
            .WithConfirmPassword("MatchingPass1")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
