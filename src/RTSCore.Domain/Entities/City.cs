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

    public IReadOnlyCollection<Building> Buildings => _buildings.AsReadOnly();

    private readonly List<Building> _buildings = [];

    public City(CityPreset cityPreset, FactionType ownerFaction)
    {
        Id = cityPreset.Id;
        Type = cityPreset.Type;
        OwnerFaction = ownerFaction;
        Population = cityPreset.CurrentPopulation;
    }

    protected City() { }

    public void GrowPopulation(float growthRate)
    {
        if (growthRate <= 0) return;

        var template = GameBalance.Cities.GetCityTemplate(Type);
        var growthBonus = (int)(Population * growthRate);
        Population = Math.Min(Population + growthBonus, template.MaxPopulation);

        if (Population < 0) Population = 0;
    }

    public void RegisterBuilding(Building building)
    {
        _buildings.Add(building);
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
}