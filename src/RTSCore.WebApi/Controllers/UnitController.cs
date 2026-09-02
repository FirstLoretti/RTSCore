using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Units.Commands;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController(IMediator mediator) : ControllerBase
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> Disband(string id)
    {
        await mediator.Send(new DisbandUnitCommand(id));

        return NoContent();
    }
}