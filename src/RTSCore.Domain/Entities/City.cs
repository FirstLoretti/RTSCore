using System.Numerics;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class City
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

    public UnitType[] GetRecruitableUnits(IReadOnlyCollection<UnitTemplate> units)
    {
        if (units.Count == 0) throw new ArgumentException("Получена пустая коллекция");

        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToHashSet();

        return [.. units
            .Where(u => u.RequiredBuilding != null && constructedBuildings.Contains(u.RequiredBuilding.Value))
            .Select(u => u.Type)];
    }

    public BuildingType[] GetConstructableBuildings(IReadOnlyCollection<BuildingType> buildings)
    {
        if (buildings.Count == 0) throw new ArgumentException("Получена пустая коллекция");

        var registredBuildings = _buildings
            .Where(b => b.IsConstructed || b.InConstructProcess)
            .Select(b => b.Type)
            .ToHashSet();

        return [.. buildings.Where(b => !registredBuildings.Contains(b))];
    }

    public Army RaiseArmy(Func<UnitType, UnitTemplate> templateFactory)
    {
        if (Governor == null)
            throw new GameRuleException("Сбор армии неовзможен без губернатора");

        var army = Army.Create(Faction, Coordinates, templateFactory(Governor.Value));
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

    public void RegisterBuilding(Building building)
    {
        if (!building.IsConstructed)
        {
            building.StartConstruct();
        }

        _buildings.Add(building);
    }

    public void TurnEnd()
    {
        foreach (var building in _buildings)
        {
            building.AdvanceConstruction();
        }
    }
}