using MediatR;
using ThePredictions.Application.Common.Interfaces;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public record DeleteSeasonCommand(int SeasonId) : IRequest, ITransactionalRequest;
