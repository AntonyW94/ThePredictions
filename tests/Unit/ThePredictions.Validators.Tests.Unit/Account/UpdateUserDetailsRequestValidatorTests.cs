using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Account;
using ThePredictions.Validators.Account;

namespace ThePredictions.Validators.Tests.Unit.Account;

public class UpdateUserDetailsRequestValidatorTests
{
    private readonly UpdateUserDetailsRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new UpdateUserDetailsRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsEmpty()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithFirstName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsTooShort()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithFirstName("J")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsTooLong()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithFirstName(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameContainsUnsafeCharacters()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithFirstName("<script>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsEmpty()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithLastName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsTooShort()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithLastName("S")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsTooLong()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithLastName(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameContainsUnsafeCharacters()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithLastName("Smith<>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameContainsAccentedCharacters()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithFirstName("Jerome")
            .WithLastName("O'Brien")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenPhoneNumberIsNull()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber(null)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPhoneNumberIsValidUkMobile()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber("07123456789")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhoneNumberIsInvalidFormat()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber("12345")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhoneNumberDoesNotStartWith07()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber("08123456789")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhoneNumberIsTooShort()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber("0712345678")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhoneNumberIsTooLong()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber("071234567890")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPhoneNumberIsEmpty()
    {
        var request = new UpdateUserDetailsRequestBuilder()
            .WithPhoneNumber("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
