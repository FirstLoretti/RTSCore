using MediatR;

using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Units.Commands;
using RTSCore.WebApi.Dtos;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UnitCreateDto dto)
    {
        var command = new CreateUnitCommand(
            dto.Id,
            dto.Type,
            dto.Faction
        );

        var id = await mediator.Send(command);

        return CreatedAtAction(
            actionName: nameof(Get),
            routeValues: new { id },
            value: new { Message = $"Юнит {id} создан на сервере и сохранён в базу данных" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var command = new GetUnitCommand(id);
        var unit = await mediator.Send(command);

        if (unit == null)
        {
            return NotFound(new
            {
                Error = $"Юнит не найден",
                Message = $"Юнита {id} нет в базе данных"
            });
        }

        var dto = new UnitResponseDto()
        {
            Id = unit.Id,
            Type = unit.Type,
            Faction = unit.Faction,
            Health = unit.Health,
            Damage = unit.Damage,
            Armor = unit.Armor,
            Level = unit.Level,
            Experience = unit.Experience
        };

        return Ok(dto);
    }

    [HttpPost("{id}/experience")]
    public async Task<IActionResult> AddExperience(string id, [FromBody] ExperienceAddDto dto)
    {
        try
        {
            var addExpCommand = new AddExperienceCommand(id, dto.Amount);
            var (level, experience) = await mediator.Send(addExpCommand);

            return Ok(new
            {
                Message = $"Юниту {id} начислен опыт",
                CurrentLevel = level,
                CurrentExperience = experience
            });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new
            {
                Error = "Юнит не найден",
                ex.Message
            });
        }
    }
}