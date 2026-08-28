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

    public void GrowPopulation(int amount)
    {
        if (amount <= 0) return;

        var template = GameBalance.Cities.GetCityTemplate(Type);
        Population = Math.Min(Population + amount, template.MaxPopulation);
    }

    public void ConstructBuilding(Building building)
    {
        _buildings.Add(building);
    }
}