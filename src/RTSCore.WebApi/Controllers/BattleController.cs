using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaign.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BattleController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("calculate")]
    public async Task<ActionResult<BattleResult>> Calculate(AutoBattleCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(result);
    }
}