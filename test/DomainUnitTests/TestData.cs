using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Numerics;

using RTSCore.Domain;
using RTSCore.Domain.AI;
using RTSCore.Domain.AI.Construction;
using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.AI.Recruitment;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Diplomacy;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Entities.Factions;

namespace DomainUnitTests;

public static class TestData
{
    public static UnitTemplate CreateUnitTemplate(int maxHealth) => new()
    {
        MaxHealth = maxHealth,
        AiUtility = 1,
        Armor = 0,
        Category = UnitCategory.Infantry,
        Cost = 1,
        Damage = 1,
        DamageGrowthRate = 1f,
        DisplayName = "Military",
        ExpKillReward = 1,
        HealthGrowthRate = 1f,
        ProductionCategory = ProductionCategory.Military,
        RequiredBuilding = BuildingType.ReqruitBarrack,
        Speed = 1,
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
        AiUtility = 1,
        AllowedCityTypes = [CityType.Settlement, CityType.Village],
        Cost = 1,
        DisplayName = "Reqruit Barrack",
        ProductionCategory = ProductionCategory.Military,
        TurnsToConstruct = 1,
        Type = BuildingType.ReqruitBarrack,
        Effects = [],
        Garrison = FrozenDictionary<UnitType, int>.Empty,
        RequiredBuildings = []
    };

    public static CityTemplate CreateCityTemplate() => new(
        CityType.Settlement,
        "Settlement",
        StartingPopulation: 1,
        PopulationForUpgrade: 2,
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

    public static DiplomacyRelation CreateDiplomacyRelation() =>
        new(FactionType.England, FactionType.France, DiplomacyConfig.InitialStanding);

    public static DiplomacyConfiguration DiplomacyConfig { get; } = new()
    {
        InitialStanding = 0,
        AcceptPeaceOfferBonus = 1,
        AcceptTradeOfferBonus = 1,
        CancelTradePenalty = -1,
        DeclareWarPenalty = -1,
    };

    public static FactionTemplate FactionTemplate { get; } = new(
        FactionType.England,
        "England",
        1000,
        CityPresets: [],
        AiPersonalityType.Conqueror
    );

    public static Faction CreateFaction() => new(CampaignId.New(), FactionTemplate);

    public static AiBudgetWeights AiBudgetWeights { get; } = new(Construction: 0.5f, Recruitment: 0.5f);
    public static AiUnitWeights AiUnitWeights { get; } = new(InfantryMultiplier: 1f, RangeInfantryMultiplier: 1f);
    public static AiBuildingWeights AiBuildingWeights { get; } = new(EconomicMultiplier: 1f, MilitaryMultiplier: 1f);
    public static AiDiplomacyWeights AiDiplomacyWeights { get; } = new(1f, 1f, 1f, 1f, 1f, 1f, 1, 1f, 1f, 1f);

    public static AiPersonality AiPersonality { get; } = new(
        AiPersonalityType.Conqueror,
        AiBudgetWeights,
        AiUnitWeights,
        AiBuildingWeights,
        AiDiplomacyWeights
    );

    public static AiFactionContext AiFactionContext { get; } = new(
        FactionType.England,
        AiPersonality,
        Gold: 1,
        CitiesCount: 1,
        MilitaryPower: 1
    );
}