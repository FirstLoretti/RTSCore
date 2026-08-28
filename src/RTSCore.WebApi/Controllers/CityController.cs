using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Cities.Commands;
using RTSCore.Domain.ValueObjects;

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

    [HttpDelete("cancelBuildingConstruction_{buildingId}")]
    public async Task<IActionResult> CancelBuildingConstruction(string buildingId)
    {
        await mediator.Send(new CancelConstructBuildingCommand(buildingId));
        return NoContent();
    }
}