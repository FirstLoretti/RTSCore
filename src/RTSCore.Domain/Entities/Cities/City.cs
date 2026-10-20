using System.Numerics;

using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Entities.Cities;

public sealed class City : AggregateRoot
{
    public CampaignId CampaignId { get; }
    public CityId Id { get; }
    public Vector2 Coordinates { get; }

    public FactionType Faction { get; private set; }
    public CityType Type { get; private set; }
    public int Population { get; private set; }

    private readonly List<Building> _buildings = [];
    public IReadOnlyCollection<Building> Buildings => _buildings;

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
        Population = template.PopulationToUpgrade;
        Coordinates = coordinates;
    }

    private City() { }

    internal static City Create(
        CampaignId campaignId,
        CityTemplate template,
        FactionType faction,
        Vector2 coordinates
    ) => new(campaignId, CityId.New(), template, faction, coordinates);

    public IReadOnlyCollection<ProductionOption<UnitType>> GetRecruitableUnits(
        IReadOnlyCollection<UnitTemplate> units
    )
    {
        var isConstructed = IsConstructed();

        return [.. units
            .Where(u => isConstructed[(int)u.RequiredBuilding])
            .Select(u => new ProductionOption<UnitType>(
                u.DisplayName,
                u.Type,
                u.Cost,
                u.TurnsToRecruit,
                u.ProductionCategory,
                u.AiUtility))];
    }

    public IReadOnlyCollection<ProductionOption<BuildingType>> GetConstructableBuildings(
        IReadOnlyCollection<BuildingTemplate> buildings
    )
    {
        var isConstructed = IsConstructed();

        return [.. buildings
            .Where(b =>
                b.AllowedCityTypes.Contains(Type) &&
                b.RequiredBuildings.All(type => isConstructed[(int)type]))
            .Select(b => new ProductionOption<BuildingType>(
                b.DisplayName,
                b.Type,
                b.Cost,
                b.TurnsToConstruct,
                b.ProductionCategory,
                b.AiUtility))];
    }

    private bool[] IsConstructed()
    {
        bool[] isConstructed = new bool[Enum.GetValues<BuildingType>().Length];

        foreach (var building in _buildings)
        {
            if (building.IsConstructed)
                isConstructed[(int)building.Type] = true;
        }

        return isConstructed;
    }

    public Army CreateArmy(UnitTemplate general) => Army.Create(CampaignId, Faction, Coordinates, general);

    public Result AssertCanRecruit(UnitTemplate unit) =>
        _buildings.Any(b => b.IsConstructed && b.Type == unit.RequiredBuilding)
            ? Result.Success()
            : Result.Failure(GameErrors.UnavailableRecruitOption);

    public void TurnEnd(BuildingCatalog catalog, CityConfiguration configuration)
    {
        float effectsValue = 0f;
        int income = 0;

        foreach (var building in _buildings)
        {
            if (!building.IsConstructed) continue;

            var template = catalog.GetBuilding(building.Type);

            foreach (var effect in template.Effects)
            {
                if (effect.Type == BuildingEffectType.PopulationGrowth)
                    effectsValue += effect.Value;

                if (effect.Type == BuildingEffectType.GoldIncome)
                    income += effect.Value;
            }
        }

        var taxIncome = (int)(Population * configuration.TaxPerCitizen);
        income += taxIncome;

        var populationGrowth = (int)(Population * effectsValue);
        Population = int.Max(0, Population + populationGrowth);

        AddDomainEvent(new CityTurnEndEvent(Faction, income));
    }

    public Result StartConstruction(BuildingTemplate building)
    {
        if (!building.AllowedCityTypes.Contains(Type))
            return Result.Failure(GameErrors.CityUnavailableConstructionOption);

        if (_buildings.Any(b => b.Type == building.Type))
            return Result.Failure(GameErrors.CantConstructBuildingDuplicate);

        var hasRequiredBuildings = building.RequiredBuildings.All(reqType =>
            _buildings.Any(b => b.Type == reqType));

        if (!hasRequiredBuildings)
            return Result.Failure(GameErrors.ChainUnavailableConstructionOption);

        _buildings.Add(Building.CreateUnderConstruction(CampaignId, building, Faction, Id));

        return Result.Success();
    }

    public Result CancelConstruction(BuildingId id)
    {
        var building = _buildings.Single(b => b.Id == id);

        if (building.IsConstructed)
            return Result.Failure(GameErrors.CantCancelAlreadyConstructed);

        _buildings.Remove(building);

        AddDomainEvent(new BuildingConstructionCanceledEvent(Faction, building.Cost));

        return Result.Success();
    }
}