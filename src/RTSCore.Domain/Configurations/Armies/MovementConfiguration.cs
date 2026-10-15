namespace RTSCore.Domain.Configurations.Armies;

public sealed record MovementConfiguration(
    int MaxMovementPoints = 100,
    int BaseUnitDistanceCost = 10
);