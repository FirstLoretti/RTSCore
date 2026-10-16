using System.Collections.Immutable;

namespace RTSCore.Domain.Entities.Armies.Combat;

public readonly record struct BattleResult(
    bool IsAttackerWon,
    bool IsDefenderWon,
    ImmutableArray<UnitBattleLog> AttackerBattleLogs,
    ImmutableArray<UnitBattleLog> DefenderBattleLogs
);