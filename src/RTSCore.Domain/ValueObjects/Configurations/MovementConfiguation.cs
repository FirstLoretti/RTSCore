namespace RTSCore.Domain.ValueObjects.Configurations;

public record MovementConfiguration(
    int MaxMovementPoints = 100,
    int BaseUnitDistanceCost = 10
);