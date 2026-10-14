namespace RTSCore.Domain.Entities.Armies.Combat;

public record BattleResult(
    bool IsAttackerWon,
    bool IsDefenderWon,
    List<BattleLog> AttackerBattleLogs,
    List<BattleLog> DefenderBattleLogs
);