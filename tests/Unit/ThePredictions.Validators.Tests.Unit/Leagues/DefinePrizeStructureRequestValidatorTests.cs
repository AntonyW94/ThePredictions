using FluentAssertions;
using FluentValidation.TestHelper;
using ThePredictions.Contracts.Leagues;
using ThePredictions.Tests.Builders.Leagues;
using ThePredictions.Validators.Leagues;
using Xunit;

namespace ThePredictions.Validators.Tests.Unit.Leagues;

public class DefinePrizeStructureRequestValidatorTests
{
    private readonly DefinePrizeStructureRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var request = new DefinePrizeStructureRequestBuilder().Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPrizeTypeIsInvalid()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithPrizeType((Domain.Common.Enumerations.PrizeType)999)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPrizeAmountIsNegative()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithPrizeAmount(-1)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldPass_WhenPrizeAmountIsZero()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithPrizeAmount(0)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenRankIsZero()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithRank(0)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenRankIsNegative()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithRank(-1)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenMultiplierIsZero()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithMultiplier(0)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenMultiplierIsNegative()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithMultiplier(-1)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPrizeDescriptionExceeds200Characters()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithPrizeDescription(new string('a', 201))
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldPass_WhenPrizeDescriptionIsNull()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithPrizeDescription(null)
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenPrizeDescriptionIs200Characters()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([new DefinePrizeSettingDtoBuilder()
                .WithPrizeDescription(new string('a', 200))
                .Build()])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldPass_WhenPrizeSettingsIsEmpty()
    {
        var request = new DefinePrizeStructureRequestBuilder()
            .WithPrizeSettings([])
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
