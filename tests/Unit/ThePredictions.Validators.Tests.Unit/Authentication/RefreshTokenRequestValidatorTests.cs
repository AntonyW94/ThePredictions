using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Authentication;
using ThePredictions.Validators.Authentication;

namespace ThePredictions.Validators.Tests.Unit.Authentication;

public class RefreshTokenRequestValidatorTests
{
    private readonly RefreshTokenRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenTokenIsValid()
    {
        var request = new RefreshTokenRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenTokenIsNull()
    {
        var request = new RefreshTokenRequestBuilder()
            .WithToken(null)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTokenIsEmptyButNotNull()
    {
        var request = new RefreshTokenRequestBuilder()
            .WithToken("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void Validate_ShouldFail_WhenTokenExceeds500Characters()
    {
        var request = new RefreshTokenRequestBuilder()
            .WithToken(new string('a', 501))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Token);
    }
}
