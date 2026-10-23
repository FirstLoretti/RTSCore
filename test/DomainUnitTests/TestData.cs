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
            new(CityType.Village, "", PopulationForUpgrade: 0, ConstructableBuildings: []),
            new(CityType.Settlement, "", PopulationForUpgrade: 0, ConstructableBuildings: [])
        ]
    );
}