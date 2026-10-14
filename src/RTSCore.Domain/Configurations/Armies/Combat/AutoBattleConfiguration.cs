namespace RTSCore.Domain.Configurations.Armies.Combat;

public record AutoBattleConfiguration(
    float WinnerCasualties = 0.3f,
    float LoserCasualties = 1f,
    float DrawCasualties = 0.5f
);