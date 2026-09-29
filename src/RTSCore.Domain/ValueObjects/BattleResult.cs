namespace RTSCore.Domain.ValueObjects;

public record BattleResult(
    bool IsAttackerWon,
    bool IsDefenderWon,
    List<BattleLog> AttackerBattleLogs,
    List<BattleLog> DefenderBattleLogs
);