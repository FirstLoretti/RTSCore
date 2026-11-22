namespace RTSCore.Domain.Entities.Armies.Combat;

public readonly record struct BattleResult(
    bool IsAttackerWon,
    bool IsDefenderWon,
    IReadOnlyList<UnitBattleLog> AttackerBattleLogs,
    IReadOnlyList<UnitBattleLog> DefenderBattleLogs
);