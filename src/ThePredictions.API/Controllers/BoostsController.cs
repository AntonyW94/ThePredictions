using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThePredictions.Application.Features.Boosts.Queries;
using ThePredictions.Application.Services.Boosts;
using ThePredictions.Contracts.Boosts;
using Swashbuckle.AspNetCore.Annotations;

namespace ThePredictions.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Boosts - Apply score multipliers to predictions")]
public class BoostsController(IBoostService boostService, IMediator mediator) : ApiControllerBase
{
    [HttpGet("available")]
    [SwaggerOperation(
        Summary = "Get available boosts",
        Description = "Returns boosts available to the user for the specified league and round. Includes remaining usage counts and eligibility status.")]
    [SwaggerResponse(200, "Available boosts retrieved successfully", typeof(List<BoostOptionDto>))]
    [SwaggerResponse(401, "Not authenticated")]
    public async Task<IActionResult> GetAvailableAsync(
        [FromQuery, SwaggerParameter("League identifier", Required = true)] int leagueId,
        [FromQuery, SwaggerParameter("Round identifier", Required = true)] int roundId,
        CancellationToken cancellationToken)
    {
        var query = new GetAvailableBoostsQuery(leagueId, roundId, CurrentUserId);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost("apply")]
    [SwaggerOperation(
        Summary = "Apply a boost to predictions",
        Description = "Applies a boost (e.g., Double Up) to the user's predictions for a specific league and round. The boost multiplies points earned.")]
    [SwaggerResponse(200, "Boost applied successfully", typeof(ApplyBoostResultDto))]
    [SwaggerResponse(400, "Boost not available or already used")]
    [SwaggerResponse(401, "Not authenticated")]
    public async Task<ActionResult<ApplyBoostResultDto>> ApplyAsync(
        [FromBody, SwaggerParameter("Boost application details", Required = true)] ApplyBoostRequest req,
        CancellationToken cancellationToken)
    {
        var result = await boostService.ApplyBoostAsync(CurrentUserId, req.LeagueId, req.RoundId, req.BoostCode, cancellationToken);
        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("user/usage")]
    [SwaggerOperation(
        Summary = "Remove a boost from predictions",
        Description = "Removes a previously applied boost from the user's predictions for a specific league and round. The boost usage is restored.")]
    [SwaggerResponse(204, "Boost removed successfully")]
    [SwaggerResponse(401, "Not authenticated")]
    public async Task<IActionResult> DeleteUserBoostUsageAsync(
        [FromQuery, SwaggerParameter("League identifier", Required = true)] int leagueId,
        [FromQuery, SwaggerParameter("Round identifier", Required = true)] int roundId,
        CancellationToken cancellationToken)
    {
        await boostService.DeleteUserBoostUsageAsync(CurrentUserId, leagueId, roundId, cancellationToken);

        return NoContent();
    }
}
