using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

using static RTSCore.Domain.Services.GameBalance;

namespace RTSCore.Tests;

public class UnitTests
{
    [Theory]
    [MemberData(nameof(TakeDamageData))]
    public void TakeDamage_ShouldDecreaseHealth_DependingOnAmount(
        int damage,
        int expectedHealth,
        bool isAlive
    )
    {
        var unit = CreateUnit();
        unit.TakeDamage(damage);

        Assert.Equal(expectedHealth, unit.Health);
        Assert.Equal(isAlive, unit.IsAlive);
    }

    [Theory]
    [MemberData(nameof(AddExperienceData))]
    public void AddExperience_ShouldLevelUpAndKeepRemainingExp_DependingOnAmount(
        int expAmount,
        int expectedLevel,
        int remainingExp
    )
    {
        var unit = CreateUnit();
        unit.AddExperience(expAmount);

        Assert.Equal(expectedLevel, unit.Level);
        Assert.Equal(remainingExp, unit.Experience);
    }

    [Fact]
    public void RecalculateStats_ShouldUpdateStats_WhenLevelUp()
    {
        var unit = CreateUnit();
        var template = Units.GetTemplate(unit.Type);

        var newHealth = Units.CalculateStat(unit.Health, template.HealthGrowthRate, 2);
        var newDamage = Units.CalculateStat(unit.Damage, template.DamageGrowthRate, 2);

        unit.AddExperience(Units.ExpToNextLevel[0]);

        Assert.Equal(newHealth, unit.Health);
        Assert.Equal(newDamage, unit.Damage);
    }

    public static TheoryData<int, int, int> AddExperienceData()
    {
        var data = new TheoryData<int, int, int>
        {
            { -50, 1, 0 },

            { Units.ExpToNextLevel[0], 2, 0 },

            { Units.ExpToNextLevel[0] + Units.ExpToNextLevel[1] + 1, 3, 1 },

            { int.MaxValue, Units.ExpToNextLevel.Length, Units.ExpToNextLevel.Last() }
        };

        return data;
    }

    public static TheoryData<int, int, bool> TakeDamageData()
    {
        var unit = CreateUnit();
        var baseHealth = Units.GetTemplate(unit.Type).MaxHealth;

        var data = new TheoryData<int, int, bool>
        {
            {-50 , baseHealth, true},
            {1, baseHealth - 1, true},
            {int.MaxValue, 0 , false}
        };

        return data;
    }

    private static Unit CreateUnit()
    {
        var unit = new Unit("england_swordman_1", FactionType.England, Units.GetTemplate(UnitType.EnglandPeasant));

        return unit;
    }
}