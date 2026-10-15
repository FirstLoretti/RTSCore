namespace RTSCore.Domain.Configurations.Armies;

public readonly record struct UnitStatsWeights(
    float Health = 1.0f,
    float Armor = 1.0f,
    float Damage = 1.5f
);