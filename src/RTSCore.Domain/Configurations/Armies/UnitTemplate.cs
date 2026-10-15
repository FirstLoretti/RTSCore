using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Armies;

public sealed record UnitTemplate(
    UnitType Type = UnitType.Peasant,
    string DisplayName = "Peasant",
    int Cost = 100,
    int MaxHealth = 50,
    int Damage = 10,
    int Armor = 0,
    int Speed = 5,
    int ExpKillReward = 10,
    float HealthGrowthRate = 1.1f,
    float DamageGrowthRate = 1.1f,
    int TurnsToRecruit = 1,
    int AiUtility = 10,
    UnitCategory Category = UnitCategory.Infantry,
    ProductionCategory ProductionCategory = ProductionCategory.Military,
    BuildingType RequiredBuilding = BuildingType.ReqruitBarrack
);