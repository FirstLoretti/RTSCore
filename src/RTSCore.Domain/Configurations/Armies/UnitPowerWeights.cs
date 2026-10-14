namespace RTSCore.Domain.Configurations.Armies;

public readonly record struct UnitPowerWeights(
    float HealthWeight = 1.0f,
    float ArmorWeight = 1.0f,
    float DamageWeight = 1.5f
);