namespace RTSCore.Domain.Configurations.Armies;

public sealed record MovementConfiguration(
    int MaxMovementPoints,
    int BaseUnitDistanceCost
);