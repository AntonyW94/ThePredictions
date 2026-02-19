using MediatR;
using ThePredictions.Contracts.Predictions;

namespace ThePredictions.Application.Features.Predictions.Queries;

public record GetPredictionPageDataQuery(int RoundId, string UserId) : IRequest<PredictionPageDto>;