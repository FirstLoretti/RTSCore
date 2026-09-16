using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;

namespace RTSCore.Domain.Entities;

public class City
{
    public CityId Id { get; init; }
    public CityType Type { get; private set; }
    public FactionType OwnerFaction { get; private set; }
    public int Population { get; private set; }
    public Coordinates Coordinates { get; private set; }
    public UnitType Governor { get; private set; } = UnitType.Knight;

    public IReadOnlyCollection<Building> Buildings => _buildings.AsReadOnly();
    private readonly List<Building> _buildings = [];

    public City(CityPreset cityPreset, FactionType ownerFaction, Coordinates? coordinates = null)
    {
        Id = cityPreset.Id;
        Type = cityPreset.Type;
        OwnerFaction = ownerFaction;
        Population = cityPreset.CurrentPopulation;
        Coordinates = coordinates ?? new Coordinates(0, 0);

        foreach (var buildingType in cityPreset.BuildingTypes)
        {
            var buildingId = new BuildingId($"building_{Id}_{buildingType}");

            _buildings.Add(Building.CreateWithCustomStatus(
                buildingId, buildingType, ownerFaction, cityPreset.Id,
                isConstructed: true, turnsToConstruct: 0
            ));
        }
    }

    private City() { }

    public UnitType[] GetAvailableRecruitOptions(IReadOnlyCollection<UnitType> allOptions)
    {
        ArgumentNullException.ThrowIfNull(allOptions);

        if (allOptions.Count == 0) return [];

        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToHashSet();

        return [..allOptions.Where(unitType =>
            {
                var unit = GameBalance.Units.GetTemplate(unitType);
                return unit.RequiredBuilding == null || constructedBuildings.Contains(unit.RequiredBuilding.Value);
            })];
    }

    public BuildingType[] GetAvailableConstructOptions(IReadOnlyCollection<BuildingType> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Count == 0) return [];

        var activeBuildings = _buildings
            .Where(b => b.IsConstructed || b.InConstructProcess)
            .Select(b => b.Type)
            .ToHashSet();

        return [.. options.Where(o => !activeBuildings.Contains(o))];
    }

    public void GrowPopulation(float growthRate)
    {
        if (growthRate <= 0) return;

        var template = GameBalance.Cities.GetCityTemplate(Type);
        var growthBonus = (int)(Population * growthRate);
        Population = Math.Min(Population + growthBonus, template.MaxPopulation);

        if (Population < 0) Population = 0;
    }

    public int CalculateTaxIncome(float taxRatePerCitizen)
    {
        return (int)(Population * taxRatePerCitizen);
    }

    public int CalculateBuildingsIncome()
    {
        return (int)Buildings
            .Where(b => b.IsConstructed)
            .Select(b => GameBalance.Buildings.GetTemplate(b.Type))
            .SelectMany(t => t.Effects)
            .Where(e => e.Type == BuildingEffectType.GoldIncome)
            .Sum(e => e.Value);
    }

    public void RegisterBuilding(Building building)
    {
        if (!building.IsConstructed)
        {
            building.StartConstruct();
            building.OnBuildingCompleted += HandleBuildingCompleted;
        }

        _buildings.Add(building);
    }

    private void HandleBuildingCompleted(Building building)
    {
        building.OnBuildingCompleted -= HandleBuildingCompleted;
    }
}