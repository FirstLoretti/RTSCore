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
    public IActionResult Create([FromBody] CreateUnitDto dto)
    {
        var unitTemplateMock = new UnitTemplate(
            Type: UnitType.EnglandSwordman,
            DisplayName: "EnglandSwordman",
            MaxHealth: 100,
            Damage: 25,
            Armor: 2,
            Speed: 5,
            ExpKillReward: 25,
            HealthGrowthRate: 1.1f,
            DamageGrowthRate: 1.15f
        );
        var unit = new Unit(dto.Id, dto.Type, unitTemplateMock, dto.Faction);

        unitRepository.Save(unit);

        return Ok(new { Message = $"Юнит {dto.Id} создан на сервере и сохранён в базу данных" });
    }
}