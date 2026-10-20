using FluentAssertions;

using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Armies.Combat;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Armies.Combat;

namespace DomainUnitTests.Entities.Armies.Combat;

public class AutoBattleCalculatorTests
{
    [Fact]
    public void Calculate_WhenAttackerStronger_ShouldReturnAttackerVictory()
    {
        var (template, calculator) = Arrange();

        var result = calculator.Calculate(
            attaсkers: [.. Enumerable.Range(0, 5).Select(_ => TestData.CreateUnit(template))],
            defenders: [TestData.CreateUnit(template)]
        );

        result.AttackerBattleLogs.Should().HaveCount(5);
        result.DefenderBattleLogs.Should().HaveCount(1);
        result.IsAttackerWon.Should().BeTrue();
        result.IsDefenderWon.Should().BeFalse();
        result.AttackerBattleLogs.Should().OnlyContain(u => u.RemainingHealth == 50);
        result.DefenderBattleLogs.Should().ContainSingle(u => u.RemainingHealth == 0);
    }

    [Fact]
    public void Calculate_WhenDefenderStronger_ShouldReturnDefenderVictory()
    {
        var (template, calculator) = Arrange();

        var result = calculator.Calculate(
            defenders: [.. Enumerable.Range(0, 5).Select(_ => TestData.CreateUnit(template))],
            attaсkers: [TestData.CreateUnit(template)]
        );

        result.AttackerBattleLogs.Should().HaveCount(1);
        result.DefenderBattleLogs.Should().HaveCount(5);
        result.IsAttackerWon.Should().BeFalse();
        result.IsDefenderWon.Should().BeTrue();
        result.AttackerBattleLogs.Should().ContainSingle(u => u.RemainingHealth == 0);
        result.DefenderBattleLogs.Should().OnlyContain(u => u.RemainingHealth == 50);
    }

    [Fact]
    public void Calculate_WhenPowerEqual_ShouldReturnDraw()
    {
        var (template, calculator) = Arrange();

        var result = calculator.Calculate(
            defenders: [.. Enumerable.Range(0, 5).Select(_ => TestData.CreateUnit(template))],
            attaсkers: [.. Enumerable.Range(0, 5).Select(_ => TestData.CreateUnit(template))]
        );

        result.AttackerBattleLogs.Should().HaveCount(5);
        result.DefenderBattleLogs.Should().HaveCount(5);
        result.IsAttackerWon.Should().BeFalse();
        result.IsDefenderWon.Should().BeFalse();
        result.AttackerBattleLogs.Should().OnlyContain(u => u.RemainingHealth == 50);
        result.DefenderBattleLogs.Should().OnlyContain(u => u.RemainingHealth == 50);
    }

    private static (UnitTemplate template, AutoBattleCalculator calculator) Arrange()
    {
        var unitTemplate = TestData.CreateUnitTemplate(maxHealth: 100);
        var catalog = new UnitCatalog([unitTemplate]);
        var config = new AutoBattleConfiguration(DamageToWinner: 0.5f, DamageToLoser: 1f, DrawDamage: 0.5f);

        var statsCalculator = new UnitStatsCalculator(TestData.UnitStatsWeights);
        var battleCalculator = new AutoBattleCalculator(catalog, config, statsCalculator);

        return (unitTemplate, battleCalculator);
    }
}