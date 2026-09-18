namespace RTSCore.Domain.ValueObjects;

public record BattleResult(
    bool IsAttackerWon,
    List<UnitBattleLog> AttackerUnitsLogs,
    List<UnitBattleLog> DefenderUnitsLogs
);