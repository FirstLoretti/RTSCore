using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.ValueObjects;

public readonly record struct BattleLog(
    UnitId UnitId,
    int DamageTaken,
    int RemainingHealth,
    bool IsAlive
);