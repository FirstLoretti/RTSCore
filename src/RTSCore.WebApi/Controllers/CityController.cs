using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Cities.Commands;
using RTSCore.Application.Cities.Queries;

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

    [HttpGet("{cityId}/getConstructionOptions")]
    public async Task<ActionResult<IEnumerable<ConstructionOptionDto>>> GetConstructionOptionsAsync(string cityId)
    {
        var result = await mediator.Send(new GetConstructionOptionsQuery(cityId));
        return Ok(result);
    }
}