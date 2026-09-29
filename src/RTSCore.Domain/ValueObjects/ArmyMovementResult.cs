namespace RTSCore.Domain.ValueObjects;

public readonly record struct ArmyMovementResult(
    ArmyId Id,
    float X,
    float Y,
    int RemainingMovementPoints
);