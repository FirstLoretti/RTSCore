using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.ValueObjects;

public record UnitTemplate(
    UnitType Type = UnitType.MercenaryKnight,
    string DisplayName = "Путевой Рыцарь",
    int Cost = 200,
    int MaxHealth = 200,
    int Damage = 100,
    int Armor = 10,
    int Speed = 5,
    int ExpKillReward = 50,
    float HealthGrowthRate = 1.1f,
    float DamageGrowthRate = 1.1f,
    int TurnsToRecruit = 1,
    UnitCategory Category = UnitCategory.Infantry,
    int AiUtility = 50,
    BuildingType? RequiredBuilding = null
) : ICatalogOption<UnitType>;