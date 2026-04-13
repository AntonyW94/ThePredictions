using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Authentication;
using ThePredictions.Validators.Authentication;

namespace ThePredictions.Validators.Tests.Unit.Authentication;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new RegisterRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsEmpty()
    {
        var request = new RegisterRequestBuilder()
            .WithFirstName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsTooShort()
    {
        var request = new RegisterRequestBuilder()
            .WithFirstName("J")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsTooLong()
    {
        var request = new RegisterRequestBuilder()
            .WithFirstName(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameContainsUnsafeCharacters()
    {
        var request = new RegisterRequestBuilder()
            .WithFirstName("<script>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsEmpty()
    {
        var request = new RegisterRequestBuilder()
            .WithLastName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsTooShort()
    {
        var request = new RegisterRequestBuilder()
            .WithLastName("S")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsTooLong()
    {
        var request = new RegisterRequestBuilder()
            .WithLastName(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameContainsUnsafeCharacters()
    {
        var request = new RegisterRequestBuilder()
            .WithLastName("Smith<>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameContainsAccentedCharacters()
    {
        var request = new RegisterRequestBuilder()
            .WithFirstName("Jerome")
            .WithLastName("O'Brien")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameContainsHyphens()
    {
        var request = new RegisterRequestBuilder()
            .WithFirstName("Mary-Jane")
            .WithLastName("Smith-Jones")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        var request = new RegisterRequestBuilder()
            .WithEmail("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var request = new RegisterRequestBuilder()
            .WithEmail("not-an-email")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        var request = new RegisterRequestBuilder()
            .WithPassword("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsTooShort()
    {
        var request = new RegisterRequestBuilder()
            .WithPassword("Short1")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsTooLong()
    {
        var request = new RegisterRequestBuilder()
            .WithPassword(new string('a', 101))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPasswordIsExactly8Characters()
    {
        var request = new RegisterRequestBuilder()
            .WithPassword("12345678")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
