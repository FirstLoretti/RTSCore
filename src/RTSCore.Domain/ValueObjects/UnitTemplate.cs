namespace RTSCore.Domain.ValueObjects;

public readonly record struct UnitTemplate(
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