namespace RTSCore.Domain.ValueObjects.Configurations;

public readonly record struct AutoBattleConfiguration(
    float WinnerCasualties = 0.3f,
    float LoserCasualties = 1f,
    float DrawCasualties = 0.5f
);