using System.IdentityModel.Tokens.Jwt;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Application.Authentication.Common;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var authResponse = await mediator.Send(command, cancellationToken);
        return Ok(authResponse);
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var authResponse = await mediator.Send(command, cancellationToken);
        return Ok(authResponse);
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        await mediator.Send(new LogoutUserCommand(userId), cancellationToken);
        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        RefreshTokensCommand command, CancellationToken cancellationToken
    )
    {
        var authResponse = await mediator.Send(command, cancellationToken);
        return Ok(authResponse);
    }
}