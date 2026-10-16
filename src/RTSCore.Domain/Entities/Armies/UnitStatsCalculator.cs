using RTSCore.Domain.Configurations.Armies;

namespace RTSCore.Domain.Entities.Armies;

public sealed class UnitStatsCalculator(UnitStatsWeights statsWeights)
{
    public static int Stat(int value, int level, float growthPercent) =>
        (int)(value + value * (growthPercent * level));

    public int Power(UnitTemplate unit, int level, int health)
    {
        var damage = Stat(unit.Damage, level, unit.DamageGrowthRate);

        return (int)(damage * statsWeights.Damage + health * statsWeights.Health);
    }
}