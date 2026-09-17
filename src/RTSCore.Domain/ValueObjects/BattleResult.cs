namespace RTSCore.Domain.ValueObjects;

public record BattleResult(
    bool IsAttackerWinner,
    List<UnitBattleLog> AttackerLog,
    List<UnitBattleLog> DefenderLog
);