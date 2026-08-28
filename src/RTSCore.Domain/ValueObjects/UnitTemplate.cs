namespace RTSCore.Domain.ValueObjects;

public record UnitTemplate(
    UnitType Type,
    string DisplayName,
    int MaxHealth,
    int Damage,
    int Armor,
    int Speed,
    int ExpKillReward,
    float HealthGrowthRate,
    float DamageGrowthRate
);