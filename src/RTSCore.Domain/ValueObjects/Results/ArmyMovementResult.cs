using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.ValueObjects.Results;

public readonly record struct ArmyMovementResult(
    ArmyId Id,
    float X,
    float Y,
    int RemainingMovementPoints
);