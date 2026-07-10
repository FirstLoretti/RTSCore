using Microsoft.AspNetCore.Mvc;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.WebApi.Dtos;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController(IUnitRepository unitRepository) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] UnitCreateDto dto)
    {
        var unit = new Unit(dto.Id, dto.Type, dto.Faction);

        unitRepository.Add(unit);
        unitRepository.Save(unit);

        return CreatedAtAction(
            actionName: nameof(Get),
            routeValues: new { id = unit.Id.Value },
            value: new { Message = $"Юнит {dto.Id} создан на сервере и сохранён в базу данных" });
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var unitId = new UnitId(id);
        var unit = unitRepository.GetUnit(unitId);

        if (unit == null)
        {
            return NotFound(new
            {
                Error = $"Сущность не найдена",
                Message = $"Юнита {id} нет в базе данных"
            });
        }

        var dto = new UnitResponseDto()
        {
            Id = unitId.Value,
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
    public IActionResult AddExperience(string id, [FromBody] ExperienceAddDto dto)
    {
        var unitId = new UnitId(id);
        var unit = unitRepository.GetUnit(unitId);

        if (unit == null)
        {
            return NotFound(new
            {
                Error = $"Сущность не найдена",
                Message = $"Юнита {id} нет в базе данных"
            });
        }

        unit.AddExperience(dto.Amount);
        unitRepository.Save(unit);

        return Ok(new
        {
            Message = $"Юниту {id} начислен опыт",
            CurrentLevel = unit.Level,
            CurrentExperience = unit.Experience
        });
    }
}