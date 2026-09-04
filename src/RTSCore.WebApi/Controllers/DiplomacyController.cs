using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaing.Commands.Diplomacy;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiplomacyController(IMediator mediator) : ControllerBase
{
    [HttpPost("offers/trade")]
    public async Task<IActionResult> SendTradeOffer(SendTradeOfferCommand command)
    {
        var offerId = await mediator.Send(command);
        return Ok(offerId);
    }

    [HttpPost("offers/peace")]
    public async Task<IActionResult> SendPeaceOffer(SendPeaceOfferCommand command)
    {
        var offerId = await mediator.Send(command);
        return Ok(offerId);
    }

    [HttpPost("offers/war")]
    public async Task<IActionResult> DeclareWar(DeclareWarCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost("offers/{id}/accept")]
    public async Task<IActionResult> AcceptOffer(Guid id)
    {
        await mediator.Send(new AcceptOfferCommand(id));
        return NoContent();
    }

    [HttpPost("offers/{id}/reject")]
    public async Task<IActionResult> RejectOffer(Guid id)
    {
        await mediator.Send(new RejectOfferCommand(id));
        return NoContent();
    }
}