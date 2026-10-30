using System.Collections.Immutable;
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
        Population = template.StartingPopulation;
        Coordinates = coordinates;
    }

    private City() { }

    internal static City Create(
        CampaignId campaignId,
        CityTemplate template,
        FactionType faction,
        Vector2 coordinates
    ) => new(campaignId, CityId.New(), template, faction, coordinates);

    public ImmutableArray<ProductionOption<UnitType>> GetRecruitableUnits(
        IReadOnlyCollection<UnitTemplate> unitTemplates
    )
    {
        var isConstructed = IsConstructed();

        return [.. unitTemplates
            .Where(u => isConstructed[(int)u.RequiredBuilding])
            .Select(u => new ProductionOption<UnitType>(
                u.DisplayName,
                u.Type,
                u.Cost,
                u.TurnsToRecruit,
                u.ProductionCategory,
                u.AiUtility))];
    }

    public ImmutableArray<ProductionOption<BuildingType>> GetConstructableBuildings(
        IReadOnlyCollection<BuildingTemplate> buildingTemplates
    )
    {
        var isConstructed = IsConstructed();

        return [.. buildingTemplates
            .Where(b =>
                b.AllowedCityTypes.Contains(Type) &&
                !isConstructed[(int)b.Type] &&
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

    public void TurnEnd(BuildingCatalog catalog, CityConfiguration configuration)
    {
        var growthEffects = 0f;
        var buildingsIncome = 0f;

        foreach (var building in _buildings)
        {
            building.AdvanceConstruction();

            if (!building.IsConstructed) continue;

            var template = catalog.GetBuilding(building.Type);

            foreach (var effect in template.Effects)
            {
                if (effect.Type == BuildingEffectType.PopulationGrowth)
                    growthEffects += effect.Value;

                if (effect.Type == BuildingEffectType.GoldIncome)
                    buildingsIncome += effect.Value;
            }
        }

        var populationGrowth = (int)(Population * growthEffects);
        Population = int.Max(0, Population + populationGrowth);

        var taxesIncome = (int)(Population * configuration.TaxPerCitizen);

        AddDomainEvent(new CityTurnEndEvent(Faction, (int)buildingsIncome, taxesIncome));
    }

    public Result<BuildingId> StartConstruction(BuildingTemplate template)
    {
        var result = CanConstruct(template);

        if (result.IsFailure) return result.Error;

        var building = Building.CreateUnderConstruction(CampaignId, template, Faction, Id);
        _buildings.Add(building);

        return building.Id;
    }

    public Result<BuildingId> ConstructInstally(BuildingTemplate template)
    {
        var result = CanConstruct(template);

        if (result.IsFailure) return result.Error;

        var building = Building.CreateConstructed(CampaignId, template, Faction, Id);
        _buildings.Add(building);

        return building.Id;
    }

    public Result CancelConstruction(BuildingId id)
    {
        var building = GetBuildingOrThrow(id);

        if (building.IsConstructed)
            return Result.Failure(GameErrors.CantCancelAlreadyConstructed);

        _buildings.Remove(building);

        AddDomainEvent(new ConstructionCanceledEvent(Faction, building.Cost));

        return Result.Success();
    }

    internal Result CanConstruct(BuildingTemplate building)
    {
        if (!building.AllowedCityTypes.Contains(Type))
            return Result.Failure(GameErrors.CityCantConstructThisBuildingType);

        if (_buildings.Any(b => b.Type == building.Type))
            return Result.Failure(GameErrors.CantConstructBuildingDuplicate);

        var hasRequiredBuildings = building.RequiredBuildings.All(reqType =>
            _buildings.Any(b => b.Type == reqType));

        return !hasRequiredBuildings
            ? Result.Failure(GameErrors.ChainUnavailableConstructionOption)
            : Result.Success();
    }

    private Building GetBuildingOrThrow(BuildingId id) => _buildings.SingleOrDefault(b => b.Id == id)
        ?? throw new InvalidOperationException($"City:{Id} does not contain it's own building:{id}");
}