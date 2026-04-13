using ThePredictions.Contracts.Predictions;

namespace ThePredictions.Tests.Builders.Predictions;

public class SubmitPredictionsRequestBuilder
{
    private int _roundId = 1;
    private List<PredictionSubmissionDto> _predictions = [new PredictionSubmissionDtoBuilder().Build()];

    public SubmitPredictionsRequestBuilder WithRoundId(int roundId)
    {
        _roundId = roundId;
        return this;
    }

    public SubmitPredictionsRequestBuilder WithPredictions(List<PredictionSubmissionDto> predictions)
    {
        _predictions = predictions;
        return this;
    }

    public SubmitPredictionsRequest Build() => new()
    {
        RoundId = _roundId,
        Predictions = _predictions
    };
}
