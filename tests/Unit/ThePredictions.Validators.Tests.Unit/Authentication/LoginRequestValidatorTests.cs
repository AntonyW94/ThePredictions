using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Authentication;
using ThePredictions.Validators.Authentication;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Authentication;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new LoginRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        var request = new LoginRequestBuilder()
            .WithEmail("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var request = new LoginRequestBuilder()
            .WithEmail("not-an-email")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        var request = new LoginRequestBuilder()
            .WithPassword("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordExceeds100Characters()
    {
        var request = new LoginRequestBuilder()
            .WithPassword(new string('a', 101))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShouldNotValidateEmailFormat_WhenEmailIsEmpty()
    {
        var request = new LoginRequestBuilder()
            .WithEmail("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Please enter your email address.");
    }
}
