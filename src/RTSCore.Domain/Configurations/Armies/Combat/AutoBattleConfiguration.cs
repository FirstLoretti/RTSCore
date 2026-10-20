namespace RTSCore.Domain.Configurations.Armies.Combat;

public sealed record AutoBattleConfiguration(
    float DamageToWinner,
    float DamageToLoser,
    float DrawDamage
);