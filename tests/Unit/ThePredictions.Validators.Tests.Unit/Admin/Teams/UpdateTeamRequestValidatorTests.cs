using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Teams;
using ThePredictions.Validators.Admin.Teams;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Admin.Teams;

public class UpdateTeamRequestValidatorTests
{
    private readonly UpdateTeamRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new UpdateTeamRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooShort()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithName("A")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooLong()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithName(new string('a', 101))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameContainsHtmlTags()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithName("<script>alert</script>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameIsEmpty()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithShortName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameIsTooShort()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithShortName("A")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameIsTooLong()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithShortName(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLogoUrlIsEmpty()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithLogoUrl("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LogoUrl);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLogoUrlIsInvalid()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithLogoUrl("not-a-url")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LogoUrl);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAbbreviationIsEmpty()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithAbbreviation("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Abbreviation);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAbbreviationIsTooShort()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithAbbreviation("MU")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Abbreviation);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAbbreviationIsTooLong()
    {
        var request = new UpdateTeamRequestBuilder()
            .WithAbbreviation("MUNU")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Abbreviation);
    }
}
