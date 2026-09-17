using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Army.Commands;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArmyController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("move")]
    public async Task<IActionResult> Move(MoveArmyCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}