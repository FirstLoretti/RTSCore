namespace RTSCore.Domain.Configurations;

public record MovementConfiguration(
    int MaxMovementPoints = 100,
    int BaseUnitDistanceCost = 10
);