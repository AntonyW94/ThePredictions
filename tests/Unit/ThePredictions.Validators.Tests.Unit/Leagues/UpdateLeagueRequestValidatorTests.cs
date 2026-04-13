using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Leagues;
using ThePredictions.Validators.Leagues;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Leagues;

public class UpdateLeagueRequestValidatorTests
{
    private readonly UpdateLeagueRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new UpdateLeagueRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooShort()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithName("AB")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooLong()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithName(new string('a', 101))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameContainsHtmlTags()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithName("<script>alert</script>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameContainsAllowedPunctuation()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithName("Smiths League (2024-25)!")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceIsNegative()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPrice(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceExceeds10000()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPrice(10001)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPriceIsZero()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPrice(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEntryDeadlineIsInThePast()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithEntryDeadlineUtc(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntryDeadlineUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForExactScoreIsZero()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPointsForExactScore(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForExactScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForExactScoreExceeds100()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPointsForExactScore(101)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForExactScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForCorrectResultIsZero()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPointsForCorrectResult(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForCorrectResult);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForCorrectResultExceeds100()
    {
        var request = new UpdateLeagueRequestBuilder()
            .WithPointsForCorrectResult(101)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForCorrectResult);
    }
}
