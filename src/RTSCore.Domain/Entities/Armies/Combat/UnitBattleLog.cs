namespace RTSCore.Domain.Entities.Armies.Combat;

public readonly record struct UnitBattleLog(
    UnitId UnitId,
    int DamageTaken
);