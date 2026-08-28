using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaing.Commands;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(IMediator mediator) : ControllerBase
{
    [HttpPost("start")]
    public async Task<IActionResult> Start(StartCampaignCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("endTurn")]
    public async Task<IActionResult> EndTurn(EndTurnCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}