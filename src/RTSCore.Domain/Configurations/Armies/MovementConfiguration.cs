namespace RTSCore.Domain.Configurations.Armies;

public record MovementConfiguration(
    int MaxMovementPoints = 100,
    int BaseUnitDistanceCost = 10
);