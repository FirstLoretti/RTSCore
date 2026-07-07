using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Tests;

public class UnitTests
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
    public void TakeDamage_ShouldDecreaseHealth_DependingOnAmount(
        int damage,
        int expectedHealth,
        bool isAlive
    )
    {
        var unit = new Unit(
            id: "england_swordman_1",
            type: UnitType.EnglandSwordman,
            template: _template,
            faction: FactionType.England
        );

        unit.TakeDamage(damage);

        Assert.Equal(expectedHealth, unit.Health);
        Assert.Equal(isAlive, unit.IsAlive);
    }

    [Theory]
    [InlineData(-50, 1, 0)]
    [InlineData(50, 2, 0)]
    [InlineData(175, 3, 25)]
    [InlineData(1000, 4, 200)]
    public void TakeExp_ShouldLevelUpAndKeepRemainingExp_DependingOnAmount(
        int expAmount,
        int expectedLevel,
        int remainingExp
    )
    {
        var unit = new Unit(
           id: "england_swordman_1",
           type: UnitType.EnglandSwordman,
           template: _template,
           faction: FactionType.England
       );

        unit.AddExperience(expAmount);

        Assert.Equal(expectedLevel, unit.Level);
        Assert.Equal(remainingExp, unit.Experience);
    }
}