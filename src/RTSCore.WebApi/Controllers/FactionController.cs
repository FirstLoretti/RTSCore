using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaign.Commands;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FactionController(IMediator mediator) : ControllerBase
{
    [HttpPost("{faction}/turn/end")]
    public async Task<IActionResult> EndTurn(FactionType faction, CancellationToken cancellationToken)
    {
        await mediator.Send(new EndTurnCommand(faction), cancellationToken);
        return NoContent();
    }
}