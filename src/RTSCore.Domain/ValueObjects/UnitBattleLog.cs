namespace RTSCore.Domain.ValueObjects;

public readonly record struct UnitBattleLog(UnitId UnitId, int DamageTaken, bool IsAlive);