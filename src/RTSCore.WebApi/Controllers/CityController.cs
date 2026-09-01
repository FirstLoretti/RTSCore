using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Cities.Commands;
using RTSCore.Application.Cities.Queries;
using RTSCore.Application.Units.Queries;
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

    [HttpGet("{cityId}/getConstructionOptions")]
    public async Task<ActionResult<IEnumerable<CityCatalogOptionDto<BuildingType>>>> GetConstructionOptionsAsync(string cityId)
    {
        var result = await mediator.Send(new GetCityConstructionOptionsQuery(cityId));
        return Ok(result);
    }

    [HttpGet("{cityId}/getRecruitOptions")]
    public async Task<ActionResult<IEnumerable<CityCatalogOptionDto<UnitType>>>> GetRecruitOptionsAsync(string cityId)
    {
        var result = await mediator.Send(new GetCityRecruitOptionsQuery(cityId));
        return Ok(result);
    }
}