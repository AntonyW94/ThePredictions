using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Tests.Builders.Leagues;

public class DefinePrizeStructureRequestBuilder
{
    private List<DefinePrizeSettingDto> _prizeSettings = [new DefinePrizeSettingDtoBuilder().Build()];

    public DefinePrizeStructureRequestBuilder WithPrizeSettings(List<DefinePrizeSettingDto> prizeSettings)
    {
        _prizeSettings = prizeSettings;
        return this;
    }

    public DefinePrizeStructureRequest Build() => new()
    {
        PrizeSettings = _prizeSettings
    };
}
