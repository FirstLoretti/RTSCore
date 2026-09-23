using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaign.ArmyMovement;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArmyController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(typeof(MoveArmyCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPost("move")]
    public async Task<IActionResult> Move(MoveArmyCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}