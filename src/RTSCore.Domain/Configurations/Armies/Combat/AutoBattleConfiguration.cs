namespace RTSCore.Domain.Configurations.Armies.Combat;

public sealed record AutoBattleConfiguration(
    float DamageToWinner = 0.3f,
    float DamageToLoser = 1f,
    float DrawDamage = 0.5f
);