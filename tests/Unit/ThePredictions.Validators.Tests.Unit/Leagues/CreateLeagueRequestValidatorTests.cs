using FluentValidation.TestHelper;
using ThePredictions.Tests.Builders.Leagues;
using ThePredictions.Validators.Leagues;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Leagues;

public class CreateLeagueRequestValidatorTests
{
    private readonly CreateLeagueRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new CreateLeagueRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithName("")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooShort()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithName("AB")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsTooLong()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithName(new string('a', 101))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameContainsHtmlTags()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithName("<script>alert</script>")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldPass_WhenNameContainsAllowedPunctuation()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithName("Smiths League (2024-25)!")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldFail_WhenSeasonIdIsZero()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithSeasonId(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeasonId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenSeasonIdIsNegative()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithSeasonId(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeasonId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceIsNegative()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPrice(-1)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceExceeds10000()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPrice(10001)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPriceIsZero()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPrice(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPriceIs10000()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPrice(10000)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_ShouldFail_WhenEntryDeadlineIsInThePast()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithEntryDeadlineUtc(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntryDeadlineUtc);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForExactScoreIsZero()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPointsForExactScore(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForExactScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForExactScoreExceeds100()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPointsForExactScore(101)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForExactScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForCorrectResultIsZero()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPointsForCorrectResult(0)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForCorrectResult);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPointsForCorrectResultExceeds100()
    {
        var request = new CreateLeagueRequestBuilder()
            .WithPointsForCorrectResult(101)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PointsForCorrectResult);
    }
}
