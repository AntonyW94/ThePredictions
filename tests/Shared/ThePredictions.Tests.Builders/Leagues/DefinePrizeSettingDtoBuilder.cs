using ThePredictions.Contracts.Leagues;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Tests.Builders.Leagues;

public class DefinePrizeSettingDtoBuilder
{
    private PrizeType _prizeType = PrizeType.Overall;
    private decimal _prizeAmount = 50.00m;
    private int _rank = 1;
    private int _multiplier = 1;
    private string? _prizeDescription = "First place";

    public DefinePrizeSettingDtoBuilder WithPrizeType(PrizeType prizeType)
    {
        _prizeType = prizeType;
        return this;
    }

    public DefinePrizeSettingDtoBuilder WithPrizeAmount(decimal prizeAmount)
    {
        _prizeAmount = prizeAmount;
        return this;
    }

    public DefinePrizeSettingDtoBuilder WithRank(int rank)
    {
        _rank = rank;
        return this;
    }

    public DefinePrizeSettingDtoBuilder WithMultiplier(int multiplier)
    {
        _multiplier = multiplier;
        return this;
    }

    public DefinePrizeSettingDtoBuilder WithPrizeDescription(string? prizeDescription)
    {
        _prizeDescription = prizeDescription;
        return this;
    }

    public DefinePrizeSettingDto Build() => new()
    {
        PrizeType = _prizeType,
        PrizeAmount = _prizeAmount,
        Rank = _rank,
        Multiplier = _multiplier,
        PrizeDescription = _prizeDescription
    };
}
