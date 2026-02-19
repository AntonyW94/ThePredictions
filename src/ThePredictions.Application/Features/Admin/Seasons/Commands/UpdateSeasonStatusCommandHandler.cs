using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public class UpdateSeasonStatusCommandHandler(ISeasonRepository seasonRepository, ICurrentUserService currentUserService) : IRequestHandler<UpdateSeasonStatusCommand>
{
    public async Task Handle(UpdateSeasonStatusCommand request, CancellationToken cancellationToken)
    {
        currentUserService.EnsureAdministrator();

        var season = await seasonRepository.GetByIdAsync(request.SeasonId, cancellationToken);
        Guard.Against.EntityNotFound(request.SeasonId, season, "Season");

        season.SetIsActive(request.IsActive);

        await seasonRepository.UpdateAsync(season, cancellationToken);
    }
}