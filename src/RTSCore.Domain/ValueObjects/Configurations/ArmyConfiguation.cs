namespace RTSCore.Domain.ValueObjects.Configurations;

public readonly record struct ArmyConfiguration(
    int MaxMovementPoints = 100,
    int MovementUnitDistanceCost = 1
);