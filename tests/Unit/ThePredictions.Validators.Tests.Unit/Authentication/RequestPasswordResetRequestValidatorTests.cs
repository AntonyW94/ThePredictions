using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Authentication;
using ThePredictions.Validators.Authentication;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Authentication;

public class RequestPasswordResetRequestValidatorTests
{
    private readonly RequestPasswordResetRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenEmailIsValid()
    {
        var request = new RequestPasswordResetRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        var request = new RequestPasswordResetRequestBuilder()
            .WithEmail("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var request = new RequestPasswordResetRequestBuilder()
            .WithEmail("not-an-email")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShouldNotValidateEmailFormat_WhenEmailIsEmpty()
    {
        var request = new RequestPasswordResetRequestBuilder()
            .WithEmail("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Please enter your email address.");
    }
}
