using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Authentication.Commands;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        string token = await mediator.Send(command, cancellationToken);
        return Ok(token);
    }
}