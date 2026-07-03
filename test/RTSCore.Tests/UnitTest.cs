using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Tests;

public class UnitTest
{
    private readonly UnitTemplate _template = new(
        Type: UnitType.EnglandSwordman,
        DisplayName: "EnglandSwordman",
        MaxHealth: 100,
        Damage: 25,
        Armor: 2,
        Speed: 5,
        ExpKillReward: 25,
        HealthGrowthRate: 1.1f,
        DamageGrowthRate: 1.15f
    );

    [Theory]
    [InlineData(-50, 100, true)]
    [InlineData(50, 50, true)]
    [InlineData(150, 0, false)]
    public void TakeDamage_ShouldProcessCorrectly(int damage, int expectedHealth, bool isAlive)
    {
        var unit = new Unit("england_swordman_1", _template, FactionType.England);

        unit.TakeDamage(damage);

        Assert.Equal(unit.Health, expectedHealth);
        Assert.Equal(unit.IsAlive, isAlive);
    }

    [Theory]
    [InlineData(-50, 1, 0)]
    [InlineData(50, 2, 0)]
    [InlineData(175, 3, 25)]
    [InlineData(1000, 4, 200)]
    public void TakeExp_ShouldProcessCorrectly(int expAmount, int expectedLevel, int remainingExp)
    {
        var unit = new Unit("england_swordman_1", _template, FactionType.England);

        unit.AddExperience(expAmount);

        Assert.Equal(expectedLevel, unit.Level);
        Assert.Equal(remainingExp, unit.Experience);
    }
}