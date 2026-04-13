using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Admin.Teams;
using ThePredictions.Validators.Admin.Teams;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Admin.Teams;

public class CreateTeamRequestValidatorTests
{
    private readonly CreateTeamRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new CreateTeamRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var request = new CreateTeamRequestBuilder()
            .WithName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooShort()
    {
        var request = new CreateTeamRequestBuilder()
            .WithName("A")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooLong()
    {
        var request = new CreateTeamRequestBuilder()
            .WithName(new string('a', 101))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameContainsHtmlTags()
    {
        var request = new CreateTeamRequestBuilder()
            .WithName("<b>Team</b>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameContainsAllowedPunctuation()
    {
        var request = new CreateTeamRequestBuilder()
            .WithName("AFC Bournemouth (2024-25)")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldNotValidateNameFormat_WhenNameIsEmpty()
    {
        var request = new CreateTeamRequestBuilder()
            .WithName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Please enter a team name.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameIsEmpty()
    {
        var request = new CreateTeamRequestBuilder()
            .WithShortName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameIsTooShort()
    {
        var request = new CreateTeamRequestBuilder()
            .WithShortName("A")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameIsTooLong()
    {
        var request = new CreateTeamRequestBuilder()
            .WithShortName(new string('a', 51))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenShortNameContainsHtmlTags()
    {
        var request = new CreateTeamRequestBuilder()
            .WithShortName("<b>Team</b>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ShortName);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLogoUrlIsEmpty()
    {
        var request = new CreateTeamRequestBuilder()
            .WithLogoUrl("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LogoUrl);
    }

    [Fact]
    public void Validate_ShouldFail_WhenLogoUrlIsInvalid()
    {
        var request = new CreateTeamRequestBuilder()
            .WithLogoUrl("not-a-url")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LogoUrl);
    }

    [Fact]
    public void Validate_ShouldPass_WhenLogoUrlIsValidHttps()
    {
        var request = new CreateTeamRequestBuilder()
            .WithLogoUrl("https://cdn.example.com/logos/team.png")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.LogoUrl);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAbbreviationIsEmpty()
    {
        var request = new CreateTeamRequestBuilder()
            .WithAbbreviation("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Abbreviation);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAbbreviationIsTooShort()
    {
        var request = new CreateTeamRequestBuilder()
            .WithAbbreviation("MU")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Abbreviation);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAbbreviationIsTooLong()
    {
        var request = new CreateTeamRequestBuilder()
            .WithAbbreviation("MUNU")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Abbreviation);
    }
}
