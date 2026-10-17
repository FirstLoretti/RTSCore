using System.Numerics;

using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Cities;

public sealed class City : AggregateRoot
{
    public CampaignId CampaignId { get; init; }
    public CityId Id { get; init; }
    public Vector2 Coordinates { get; init; }

    public FactionType Faction { get; private set; }
    public CityType Type { get; private set; }
    public int Population { get; private set; }
    public UnitType? Governor { get; private set; }

    public IReadOnlyList<Building> Buildings => _buildings.AsReadOnly();
    private readonly List<Building> _buildings = [];

    private City(
        CampaignId campaignId,
        CityId cityId,
        CityTemplate template,
        FactionType faction,
        Vector2 coordinates
    )
    {
        CampaignId = campaignId;
        Id = cityId;
        Type = template.Type;
        Faction = faction;
        Population = template.MaxPopulation;
        Coordinates = coordinates;
        Governor = template.Governor;
    }

    private City() { }

    internal static City Create(
        CampaignId campaignId,
        CityTemplate template,
        FactionType faction,
        Vector2 coordinates
    )
    {
        var city = new City(campaignId, CityId.New(), template, faction, coordinates);
        city.AddDomainEvent(new CityCreatedEvent(faction));

        return city;
    }

    public IReadOnlyCollection<UnitTemplate> GetRecruitableUnits(
        IReadOnlyCollection<UnitTemplate> templates
    )
    {
        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToList();

        return [.. templates
            .Where(t => t.RequiredBuilding != null && constructedBuildings.Contains(t.RequiredBuilding))
        ];
    }

    public IReadOnlyCollection<BuildingTemplate> GetConstructableBuildings(
        IReadOnlyCollection<BuildingTemplate> templates
    )
    {
        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToList();

        return [.. templates
            .Where(t =>
                !_buildings.Any(b => t.Type == t.Type)
                && t.RequiredBuildings.All(reqType => constructedBuildings.Contains(reqType))
            )];
    }

    public Army CreateArmy(IReadOnlyCollection<UnitTemplate> templates)
    {
        if (Governor == null) throw new NullReferenceException();

        var governor = templates.FirstOrDefault(t => t.Type == Governor); //

        var army = Army.Create(CampaignId, Faction, Coordinates, governor!);
        Governor = null;

        return army;
    }

    public void AssertCanRecruit(UnitTemplate unit)
    {
        if (Governor == null) throw new NullReferenceException();

        var hasRequiredBuilding = _buildings.Any(b => b.IsConstructed && b.Type == unit.RequiredBuilding);
        if (!hasRequiredBuilding)
            throw new NullReferenceException();
    }

    public void GrowPopulation(float growthRate, CityTemplate template)
    {
        var growth = (int)(Population * growthRate);
        Population = int.Clamp(Population + growth, 0, template.MaxPopulation);
    }

    public void StartConstruction(BuildingTemplate template)
    {
        if (!template.AllowedCityTypes.Any(t => t == Type))
            throw new NullReferenceException();

        if (_buildings.Any(b => b.Type == template.Type))
            throw new NullReferenceException();

        if (template.RequiredBuildings != null)
        {
            var hasRequiredBuildings = template.RequiredBuildings.All(reqType =>
                _buildings.Any(b => b.Type == reqType));

            if (!hasRequiredBuildings)
                throw new NullReferenceException();
        }

        _buildings.Add(Building.CreateUnderConstruction(CampaignId, template, Faction, Id));
    }

    public void CancelConstruction(BuildingId id)
    {
        var building = _buildings.FirstOrDefault(b => b.Id == id)
            ?? throw new NullReferenceException("Здания нет в городе");

        if (!building.IsConstructed)
            throw new NullReferenceException("Нельзя отменить построенное здание");

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
            ?? throw new NullReferenceException("Шаблон грода не найден");

        var constructedTemplates = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => buildingTemplates.FirstOrDefault(t => t.Type == b.Type)
                ?? throw new NullReferenceException("Шаблон здания не найден"))
            .ToList();

        income = CityEconomyCalculator.CalculateTurnEndIncome(
            cityTemplate,
            Population,
            constructedTemplates
        );
    }
}