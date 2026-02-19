using MediatR;
using ThePredictions.Application.Common.Interfaces;
using ThePredictions.Contracts.Predictions;

namespace ThePredictions.Application.Features.Predictions.Commands;

public record SubmitPredictionsCommand(
    string UserId,
    int RoundId,
    IEnumerable<PredictionSubmissionDto> Predictions) : IRequest, ITransactionalRequest;