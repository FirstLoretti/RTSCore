namespace RTSCore.Domain.ValueObjects.Results;

public record BattleResult(
    bool IsAttackerWon,
    bool IsDefenderWon,
    List<BattleLog> AttackerBattleLogs,
    List<BattleLog> DefenderBattleLogs
);