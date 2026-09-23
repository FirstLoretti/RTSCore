namespace RTSCore.Domain.ValueObjects;

public record BattleResult(
    bool IsAttackerWon,
    List<UnitBattleLog> AttackerBattleLogs,
    List<UnitBattleLog> DefenderBattleLogs
);