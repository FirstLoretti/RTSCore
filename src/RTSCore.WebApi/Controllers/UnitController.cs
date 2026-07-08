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
        try
        {
            var template = Units.GetTemplate(dto.Type);

            var unit = new Unit(dto.Id, dto.Type, template, dto.Faction);

            unitRepository.Save(unit);

            return Ok(new { Message = $"Юнит {dto.Id} создан на сервере и сохранён в базу данных" });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new
            {
                Error = "Ошибка валидации игрового баланса",
                Details = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var unitId = new UnitId(id);
        var unit = unitRepository.GetUnit(unitId);

        if (unit == null) return NotFound(new { Message = $"Юнит {id} не найден в базе данных" });

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
}