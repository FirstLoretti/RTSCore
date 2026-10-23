using FluentAssertions;

using RTSCore.Domain.Entities.Armies;

namespace DomainUnitTests.Entities.Armies;

public class UnitStatsCalculatorTests
{

    [Theory]
    [InlineData(10, 0, 1f, 10)]
    [InlineData(10, 1, 1f, 20)]
    public void Stat_ShouldReturnExpectedValue(int value, int level, float growthPercent, int expectedValue)
    {
        var stat = UnitStatsCalculator.Stat(value, level, growthPercent);

        stat.Should().Be(expectedValue);
    }

    [Fact]
    public void Power_ShouldReturnExpectedValue()
    {
        var template = TestData.CreateUnitTemplate(maxHealth: 100) with { Damage = 10 };
        var statsWeights = TestData.UnitStatsWeights with { Health = 1.5f, Damage = 1f };
        var calculator = new UnitStatsCalculator(statsWeights);

        var power = calculator.Power(template, level: 0, health: 100);

        power.Should().Be(160);
    }
}