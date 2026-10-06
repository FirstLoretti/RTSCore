using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Campaign.CityConstruction.CancelConstruction;
using RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;
using RTSCore.Application.Campaign.CityConstruction.StartConstruction;
using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.ValueObjects.Common;

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

    [HttpPost("trainUnit")]
    public async Task<IActionResult> TrainUnit(RecruitRegularUnitCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("cancelBuildingConstruction_{cityId}_{buildingId}")]
    public async Task<IActionResult> CancelBuildingConstruction(string cityId, string buildingId)
    {
        await mediator.Send(new CancelConstructBuildingCommand(cityId, buildingId));
        return NoContent();
    }

    [HttpDelete("cancelUnitRecruiting_{unitId}")]
    public async Task<IActionResult> CancelUnitRecruiting(string unitId)
    {
        await mediator.Send(new CancelRecruitUnitCommand(unitId));
        return NoContent();
    }

    [HttpGet("{cityId}/getConstructionOptions")]
    public async Task<ActionResult<IEnumerable<ProductionOption>>> GetConstructionOptionsAsync(string cityId)
    {
        var result = await mediator.Send(new GetCityConstructionOptionsQuery(cityId));
        return Ok(result);
    }

    [HttpGet("{cityId}/getRecruitOptions")]
    public async Task<ActionResult<IEnumerable<ProductionOption>>> GetRecruitOptionsAsync(string cityId)
    {
        var result = await mediator.Send(new GetCityRecruitOptionsQuery(cityId));
        return Ok(result);
    }
}