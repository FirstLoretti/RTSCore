namespace RTSCore.Domain.Entities.Armies.Combat;

public readonly record struct BattleLog(
    UnitId UnitId,
    int DamageTaken,
    int RemainingHealth,
    bool IsAlive
);