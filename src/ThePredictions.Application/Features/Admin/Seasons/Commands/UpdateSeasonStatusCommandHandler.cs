using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public class UpdateSeasonStatusCommandHandler : IRequestHandler<UpdateSeasonStatusCommand>
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateSeasonStatusCommandHandler(ISeasonRepository seasonRepository, ICurrentUserService currentUserService)
    {
        _seasonRepository = seasonRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateSeasonStatusCommand request, CancellationToken cancellationToken)
    {
        _currentUserService.EnsureAdministrator();

        var season = await _seasonRepository.GetByIdAsync(request.SeasonId, cancellationToken);
        Guard.Against.EntityNotFound(request.SeasonId, season, "Season");

        season.SetIsActive(request.IsActive);

        await _seasonRepository.UpdateAsync(season, cancellationToken);
    }
}