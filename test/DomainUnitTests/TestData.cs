using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Numerics;

using RTSCore.Domain;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace DomainUnitTests;

public static class TestData
{
    public static UnitTemplate CreateUnitTemplate(int maxHealth) => new()
    {
        MaxHealth = maxHealth,
        AiUtility = 0,
        Armor = 0,
        Category = UnitCategory.Infantry,
        Cost = 0,
        Damage = 0,
        DamageGrowthRate = 0f,
        DisplayName = "",
        ExpKillReward = 0,
        HealthGrowthRate = 0f,
        ProductionCategory = ProductionCategory.Military,
        RequiredBuilding = BuildingType.ReqruitBarrack,
        Speed = 0,
        TurnsToRecruit = 1,
        Type = UnitType.Peasant
    };

    public static UnitStatsWeights UnitStatsWeights { get; } = new(Damage: 1f, Health: 1f);

    public static MovementConfiguration MovementConfiguration { get; } = new(BaseUnitDistanceCost: 10, MaxMovementPoints: 100);

    public static Unit CreateUnit(UnitTemplate template) =>
        Unit.CreateTraining(CampaignId.Empty, FactionType.England, template, ArmyId.Empty);

    public static Army CreateArmy() =>
        Army.Create(CampaignId.New(), FactionType.England, Vector2.Zero, CreateUnitTemplate(maxHealth: 100));

    public static BuildingTemplate CreateBuildingTemplate() => new()
    {
        AiUtility = 0,
        AllowedCityTypes = [CityType.Settlement, CityType.Village],
        Cost = 0,
        DisplayName = "",
        ProductionCategory = ProductionCategory.Military,
        TurnsToConstruct = 1,
        Type = BuildingType.ReqruitBarrack,
        Effects = [],
        Garrison = FrozenDictionary<UnitType, int>.Empty,
        RequiredBuildings = []
    };

    public static CityTemplate CreateCityTemplate() => new(
        CityType.Settlement,
        "",
        StartingPopulation: 0,
        PopulationForUpgrade: 0,
        ConstructableBuildings:
        [
            BuildingType.ReqruitBarrack,
            BuildingType.MilitiaBarrack,
            BuildingType.Market,
            BuildingType.BigMarket,
            BuildingType.CultivatedField,
            BuildingType.CultivatedFieldsComplex
        ]
    );

    public static CityConfiguration CityConfiguration { get; } = new(GrowthRate: 1f, TaxPerCitizen: 1f);

    public static Building CreateBuilding(BuildingTemplate template) =>
        Building.CreateUnderConstruction(CampaignId.New(), template, FactionType.England, CityId.Empty);

    public static City CreateCity(CityTemplate template) =>
        City.Create(CampaignId.New(), template, FactionType.England, Vector2.Zero);

    private static ImmutableArray<CityPreset> EnglandCityPresets { get; } =
    [
        new(CityType.Settlement, DisplayName: "York", Vector2.Zero),
    ];

    private static ImmutableArray<CityPreset> FranceCityPresets { get; } =
    [
        new(CityType.Settlement, DisplayName: "Paris", Vector2.One),
    ];

    private static ImmutableArray<CityPreset> SpainCityPresets { get; } =
    [
        new(CityType.Village, DisplayName: "Madrid", new(2f, 2f)),
        new(CityType.Village, DisplayName: "Seville", new(3f, 3f))
    ];

    public static FactionCatalog FactionCatalog { get; } = new(
        [
            new(FactionType.England, "England", 1000, EnglandCityPresets, AiPersonalityType.Conqueror),
            new(FactionType.France, "France", 1000, FranceCityPresets, AiPersonalityType.Conqueror),
            new(FactionType.Spain, "Spain", 1000, SpainCityPresets, AiPersonalityType.Defender)
        ]
    );

    public static CityCatalog CityCatalog { get; } = new(
        [
            CreateCityTemplate() with {Type = CityType.Village},
            CreateCityTemplate() with {Type = CityType.Settlement}
        ]
    );

    public static BuildingCatalog BuildingCatalog { get; } = new(
        [
            CreateBuildingTemplate() with
            {
                TurnsToConstruct = 1,
                Type = BuildingType.ReqruitBarrack
            },
            CreateBuildingTemplate() with
            {
                TurnsToConstruct = 1,
                Type = BuildingType.MilitiaBarrack,
                RequiredBuildings = [BuildingType.ReqruitBarrack]
            },
            CreateBuildingTemplate() with
            {
                TurnsToConstruct = 1,
                Type = BuildingType.Market,
                Effects = [new(BuildingEffectType.GoldIncome, 1000)]
            },
            CreateBuildingTemplate() with
            {
                TurnsToConstruct = 1,
                Type = BuildingType.BigMarket,
                RequiredBuildings = [BuildingType.Market],
                Effects = [new(BuildingEffectType.GoldIncome, 2000)]
            },
            CreateBuildingTemplate() with
            {
                TurnsToConstruct = 1,
                Type = BuildingType.CultivatedField,
                Effects = [new(BuildingEffectType.PopulationGrowth, 0.25f)]
            },
            CreateBuildingTemplate() with
            {
                TurnsToConstruct = 1,
                Type = BuildingType.CultivatedFieldsComplex,
                RequiredBuildings = [BuildingType.CultivatedField],
                Effects = [new(BuildingEffectType.PopulationGrowth, 0.5f)]
            }
        ]
    );
}