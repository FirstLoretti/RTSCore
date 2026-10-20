using RTSCore.Domain;
using RTSCore.Domain.Configurations.Armies;
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
        TurnsToRecruit = 0,
        Type = UnitType.Peasant
    };

    public static Unit CreateUnit(UnitTemplate template) =>
        Unit.CreateReady(CampaignId.Empty, FactionType.England, template, ArmyId.Empty);

    public static UnitStatsWeights UnitStatsWeights { get; } = new() { Damage = 1f, Health = 1f };
}