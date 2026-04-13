using FluentValidation.TestHelper;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Tests.Builders.Admin.Matches;
using ThePredictions.Validators.Admin.Matches;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Admin.Matches;

public class MatchResultDtoValidatorTests
{
    private readonly MatchResultDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var dto = new MatchResultDtoBuilder().Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchIdIsZero()
    {
        var dto = new MatchResultDtoBuilder()
            .WithMatchId(0)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMatchIdIsNegative()
    {
        var dto = new MatchResultDtoBuilder()
            .WithMatchId(-1)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MatchId);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeScoreIsNegative()
    {
        var dto = new MatchResultDtoBuilder()
            .WithHomeScore(-1)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenHomeScoreExceeds9()
    {
        var dto = new MatchResultDtoBuilder()
            .WithHomeScore(10)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldPass_WhenHomeScoreIsZero()
    {
        var dto = new MatchResultDtoBuilder()
            .WithHomeScore(0)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldPass_WhenHomeScoreIs9()
    {
        var dto = new MatchResultDtoBuilder()
            .WithHomeScore(9)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.HomeScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayScoreIsNegative()
    {
        var dto = new MatchResultDtoBuilder()
            .WithAwayScore(-1)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AwayScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAwayScoreExceeds9()
    {
        var dto = new MatchResultDtoBuilder()
            .WithAwayScore(10)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.AwayScore);
    }

    [Fact]
    public void Validate_ShouldFail_WhenStatusIsInvalid()
    {
        var dto = new MatchResultDtoBuilder()
            .WithStatus((MatchStatus)999)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_ShouldPass_WhenStatusIsScheduled()
    {
        var dto = new MatchResultDtoBuilder()
            .WithStatus(MatchStatus.Scheduled)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_ShouldPass_WhenStatusIsPostponed()
    {
        var dto = new MatchResultDtoBuilder()
            .WithStatus(MatchStatus.Postponed)
            .Build();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }
}
