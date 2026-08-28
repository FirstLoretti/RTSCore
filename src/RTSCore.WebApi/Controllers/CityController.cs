using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Buildings.Commands;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CityController(IMediator mediator) : ControllerBase
{
    [HttpPost("constructBuilding")]
    public async Task<IActionResult> ConstructBuilding(ConstructBuildingCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}