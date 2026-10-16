namespace RTSCore.Domain.Entities.Armies.Combat;

public readonly record struct BattleResult(
    bool IsAttackerWon,
    bool IsDefenderWon,
    IReadOnlyCollection<UnitBattleLog> AttackerBattleLogs,
    IReadOnlyCollection<UnitBattleLog> DefenderBattleLogs
);