using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaing.Commands.Diplomacy;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiplomacyController(IMediator mediator) : ControllerBase
{
    [HttpPost("offers/{id}/accept")]
    public async Task<IActionResult> AcceptOffer(Guid id)
    {
        await mediator.Send(new AcceptOfferCommand(id));
        return NoContent();
    }
}