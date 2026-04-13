using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Leagues;
using ThePredictions.Validators.Leagues;

namespace ThePredictions.Validators.Tests.Unit.Leagues;

public class JoinLeagueRequestValidatorTests
{
    private readonly JoinLeagueRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new JoinLeagueRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEntryCodeIsEmpty()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntryCode);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEntryCodeIsTooShort()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("ABC12")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntryCode);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEntryCodeIsTooLong()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("ABC1234")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntryCode);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEntryCodeContainsSpecialCharacters()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("ABC!@#")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntryCode);
    }

    [Fact]
    public void Validate_ShouldPass_WhenEntryCodeIsLowerCase()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("abc123")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenEntryCodeIsAllNumbers()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("123456")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenEntryCodeIsAllLetters()
    {
        var request = new JoinLeagueRequestBuilder()
            .WithEntryCode("ABCDEF")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
