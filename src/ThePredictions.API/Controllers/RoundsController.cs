using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThePredictions.Application.Features.Dashboard.Queries;
using ThePredictions.Contracts.Admin.Rounds;
using Swashbuckle.AspNetCore.Annotations;

namespace ThePredictions.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Rounds - View round and match information")]
public class RoundsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{roundId:int}/matches-data")]
    [SwaggerOperation(
        Summary = "Get matches for a round",
        Description = "Returns all matches in the specified round with team details, kick-off times, and current scores.")]
    [SwaggerResponse(200, "Matches retrieved successfully", typeof(IEnumerable<MatchInRoundDto>))]
    [SwaggerResponse(401, "Not authenticated")]
    public async Task<ActionResult<IEnumerable<MatchInRoundDto>>> GetMatchesForRoundAsync(
        [SwaggerParameter("Round identifier")] int roundId,
        CancellationToken cancellationToken)
    {
        var query = new GetMatchesForRoundQuery(roundId);
        return Ok(await mediator.Send(query, cancellationToken));
    }
}
