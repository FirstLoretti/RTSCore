using System.Numerics;

using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services.Economy;
using RTSCore.Domain.ValueObjects.Common;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Events;
using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Entities.Campaign;

public sealed class City : AggregateRoot
{
    public CampaignId CampaignId { get; }

    public CityId Id { get; }
    public FactionType Faction { get; }
    public Vector2 Coordinates { get; }
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

    public static City Create(
        CampaignId campaignId,
        CityTemplate template,
        FactionType faction,
        Vector2 coordinates
    ) => new(campaignId, CityId.New(), template, faction, coordinates);

    public IReadOnlyCollection<ProductionOption> GetRecruitableUnits(
        IReadOnlyCollection<UnitTemplate> templates
    )
    {
        var constructedBuildings = _buildings
            .Where(b => b.IsConstructed)
            .Select(b => b.Type)
            .ToList();

        return [.. templates
            .Where(t => t.RequiredBuilding != null && constructedBuildings.Contains(t.RequiredBuilding.Value))
            .Select(t => new ProductionOption(
                Name: t.DisplayName,
                Cost: t.Cost,
                TurnsToConstruct: t.TurnsToRecruit,
                Category: t.ProductionCategory
            ))];
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
        if (Governor == null)
            throw new GameRuleException("Сбор армии невозможен без губернатора");

        var governor = templates.FirstOrDefault(t => t.Type == Governor)
            ?? throw new GameRuleException("Нет шаблона с типом губернатора");

        var army = Army.Create(CampaignId, Faction, Coordinates, governor);
        Governor = null;

        return army;
    }

    public void AssertCanRecruit(UnitTemplate unit)
    {
        if (Governor == null)
            throw new GameRuleException("Найм невозможен без губернатора");

        var hasRequiredBuilding = _buildings.Any(b => b.IsConstructed && b.Type == unit.RequiredBuilding);
        if (!hasRequiredBuilding)
            throw new GameRuleException("Нет требуемого здания для найма");
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

        _buildings.Add(Building.CreateUnderConstruction(CampaignId, template, Faction, Id));
    }

    public void CancelConstruction(BuildingId id)
    {
        var building = _buildings.FirstOrDefault(b => b.Id == id)
            ?? throw new NotFoundException("Здания нет в городе");

        if (!building.IsConstructed)
            throw new GameRuleException("Нельзя отменить построенное здание");

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