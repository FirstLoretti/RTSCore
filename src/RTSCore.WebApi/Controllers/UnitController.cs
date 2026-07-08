using Microsoft.AspNetCore.Mvc;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.WebApi.Dtos;

using static RTSCore.Domain.Services.GameBalance;

namespace RTSCore.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController(IUnitRepository unitRepository) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] UnitCreateDto dto)
    {
        UnitTemplate template;
        try
        {
            template = Units.GetTemplate(dto.Type);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new
            {
                Error = "Ошибка валидации игрового баланса",
                Details = ex.Message
            });
        }

        var unit = new Unit(dto.Id, dto.Type, template, dto.Faction);

        unitRepository.Add(unit);
        unitRepository.Save(unit);

        return Ok(new { Message = $"Юнит {dto.Id} создан на сервере и сохранён в базу данных" });
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var unitId = new UnitId(id);
        var unit = unitRepository.GetUnit(unitId);

        if (unit == null) return NotFound(new { Message = $"Юнита {id} нет в базе данных" });

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

        if (unit == null) return NotFound(new { Message = $"Юнита {id} нет в базе данных" });

        unit.AddExperience(dto.Amount);
        unitRepository.Save(unit);

        return Ok(new
        {
            Messange = $"Юниту {id} начислен опыт",
            CurrentLevel = unit.Level,
            CurrentExperience = unit.Experience
        });
    }
}