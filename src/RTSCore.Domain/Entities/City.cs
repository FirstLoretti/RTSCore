using System.Numerics;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Events;

namespace RTSCore.Domain.Entities;

public class City : AggregateRoot
{
    public CityId Id { get; init; }
    public CityType Type { get; private set; }
    public FactionType Faction { get; private set; }
    public int Population { get; private set; }
    public Vector2 Coordinates { get; private set; }
    public UnitType? Governor { get; private set; }

    public IReadOnlyList<Building> Buildings => _buildings.AsReadOnly();
    private readonly List<Building> _buildings = [];

    private City(
        CityId id,
        CityType type,
        Vector2 coordinates,
        FactionType faction,
        int population,
        UnitType? governor
    )
    {
        Id = id;
        Type = type;
        Faction = faction;
        Population = population;
        Coordinates = coordinates;
        Governor = governor;
    }

    public static City Create(
        CityType type,
        Vector2 coordinates,
        FactionType faction,
        int population,
        UnitType governor
    )
    {
        var id = $"city_{Guid.NewGuid():N}";
        return new(id, type, coordinates, faction, population, governor);
    }

    public static City CreateEmpty(
       CityType type,
       Vector2 coordinates,
       FactionType faction
    )
    {
        var id = $"city_{Guid.NewGuid():N}";
        return new(id, type, coordinates, faction, 0, null);
    }

    public IReadOnlyCollection<ProductionOption> GetRecruitableUnits(
        IReadOnlyCollection<UnitTemplate> templates
    )
    {
        if (templates.Count == 0) throw new ArgumentException("Получена пустая коллекция");

        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToList();

        return [.. templates
            .Where(t => t.RequiredBuilding == null || constructedBuildings.Contains(t.RequiredBuilding.Value))
            .Select(t => new ProductionOption(
                Name: t.DisplayName,
                Cost: t.Cost,
                TurnsToConstruct: t.TurnsToRecruit
            ))];
    }

    public IReadOnlyCollection<ProductionOption> GetConstructableBuildings(
        IReadOnlyCollection<BuildingTemplate> templates
    )
    {
        if (templates.Count == 0) throw new ArgumentException("Получена пустая коллекция");

        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToList();

        var registredBuildings = _buildings
            .Where(b => b.IsConstructed || b.InConstructProcess)
            .Select(b => b.Type)
            .ToList();

        var availableTemplates = templates
            .Where(t => !registredBuildings.Contains(t.Type))
            .Where(t => t.RequiredBuildings.All(reqType => constructedBuildings.Contains(reqType)))
            .ToList();

        return [.. availableTemplates
            .Select(t => new ProductionOption(
                Name: t.DisplayName,
                Cost: t.Cost,
                TurnsToConstruct: t.TurnsToConstruct
            ))];
    }

    public Army RaiseArmy(Func<UnitType, UnitTemplate> getTemplate)
    {
        if (Governor == null)
            throw new GameRuleException("Сбор армии невозможен без губернатора");

        var army = Army.Create(Faction, Coordinates, getTemplate(Governor.Value));
        Governor = null;

        return army;
    }

    internal void RecruitUnit(Army army, UnitTemplate unit)
    {

        if (army.Faction != Faction)
            throw new GameRuleException("Фракция армии отличается от фракции города");

        if (Governor == null)
            throw new GameRuleException("Найм невозможен без губернатора");

        if (_buildings.Any(b => b.IsConstructed && b.Type == unit.RequiredBuilding))
            throw new GameRuleException("Нет здания для найма");

        army.RecruitUnit(unit);
    }

    public void GrowPopulation(float growthRate, CityTemplate template)
    {
        var growth = (int)(Population * growthRate);
        Population = int.Clamp(Population + growth, 0, template.MaxPopulation);
    }

    public void StartConstruction(BuildingTemplate template)
    {
        if (!template.AllowedCityTypes.Any(t => t == Type))
            throw new GameRuleException("Здание недоступно для этого типа города");

        if (_buildings.Any(b => b.Type == template.Type))
            throw new GameRuleException("Здание такого типа уже построено");

        if (template.RequiredBuildings != null)
        {
            var hasRequiredBuildings = template.RequiredBuildings.All(reqType =>
                _buildings.Any(b => b.Type == reqType));

            if (!hasRequiredBuildings)
                throw new GameRuleException("Не все требуемые здания построены");
        }

        _buildings.Add(Building.CreateUnderConstruction(template, Faction, Id));
    }

    public void CancelConstruction(BuildingId id)
    {
        var building = _buildings.FirstOrDefault(b => b.Id == id)
            ?? throw new NotFoundException("Здание не существует");

        if (!building.InConstructProcess)
            throw new GameRuleException("Нельзя отменить не строящееся здание");

        _buildings.Remove(building);
        AddDomainEvent(new BuildingConstructionCanceledEvent(building.Cost, Faction));
    }

    public void TurnEnd(
        IReadOnlyCollection<CityTemplate> cityTemplates,
        IReadOnlyCollection<BuildingTemplate> buildingTemplates,
        out int income
    )
    {
        foreach (var building in _buildings)
        {
            building.AdvanceConstruction();
        }

        var cityTemplate = cityTemplates.FirstOrDefault(t => t.Type == Type)
            ?? throw new NotFoundException("Шаблон грода не найден");

        var constructedTemplates = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => buildingTemplates.FirstOrDefault(t => t.Type == b.Type)
                ?? throw new NotFoundException("Шаблон здания не найден"))
            .ToList();

        income = CityEconomyCalculator.CalculateTurnEndIncome(
            cityTemplate,
            Population,
            constructedTemplates
        );
    }
}