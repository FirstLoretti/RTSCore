using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public static class UnitStatsCalculator
{
    public static int CalculateStat(int value, int level, float growthRate) =>
        (int)float.Round(value * float.Pow(growthRate, level), MidpointRounding.AwayFromZero);

    public static int CalculateCurrentPower(UnitTemplate unit, int level, int health)
    {
        if (health <= 0 || unit.MaxHealth <= 0) throw new ArgumentException("Здоровье юнита не может быть <= 0");

        var damage = CalculateStat(unit.Damage, level, unit.DamageGrowthRate);
        var healthPercentage = health / (float)unit.MaxHealth;

        return (int)float.Round(damage * healthPercentage, MidpointRounding.AwayFromZero);
    }
}