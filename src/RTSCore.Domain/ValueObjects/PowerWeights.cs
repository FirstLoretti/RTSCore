namespace RTSCore.Domain.ValueObjects;

public readonly record struct PowerWeights(
    float HealthWeight = 1.0f,
    float ArmorWeight = 1.0f,
    float DamageWeight = 1.5f
);