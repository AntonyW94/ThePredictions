using MediatR;
using ThePredictions.Application.Common.Interfaces;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public record SendScheduledRemindersCommand : IRequest, ITransactionalRequest;