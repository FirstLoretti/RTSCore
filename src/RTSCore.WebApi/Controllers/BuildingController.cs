using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Buildings.Commands;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildingController(IMediator mediator) : ControllerBase
{
    [HttpPost("train")]
    public async Task<IActionResult> Train(TrainUnitCommand command)
    {
        await mediator.Send(command);

        return NoContent();
    }
}