namespace RTSCore.Domain.Entities.Armies;

public readonly record struct ArmyMovementResult(
    ArmyId Id,
    float X,
    float Y,
    int RemainingMovementPoints
);