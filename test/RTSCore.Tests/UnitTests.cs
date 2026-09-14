using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

using static RTSCore.Domain.Services.GameBalance;

namespace RTSCore.Tests;

/*
public class UnitTests
{
    [Theory(Skip = "Логика Unit.cs будет изменена")]
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

    [Theory(Skip = "Логика Unit.cs будет изменена")]
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

    [Fact(Skip = "Логика Unit.cs будет изменена")]
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

            { 100, 2, 0 },

            { 250, 3, 1 },

            { 9999, 5,1 }
        };

        return data;
    }

    public static TheoryData<int, int, bool> TakeDamageData()
    {
        var baseHealth = 100;

        return new TheoryData<int, int, bool>
        {
            {-50 , baseHealth, true},
            {1, baseHealth - 1, true},
            {int.MaxValue, 0 , false}
        };
    }

    private static Unit CreateUnit()
    {
        var template = new UnitTemplate(UnitType.Peasant, "Unit", 100, 100, 100, 100, 100, 10, 10, 10, 1);
        return new Unit("id", FactionType.England, template);
    }
}
*/
