using FluentAssertions;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

using Unit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Tests.UnitTests.Domain.Services;

public class AutoBattleCalculatorTests
{
    private readonly UnitTemplate[] _templates = [
        new(UnitType.Peasant, "Peasant", 1, 100 , 10, 10, 10, 1, 1, 1, 1, UnitCategory.Infantry,1),
        new(UnitType.Knight, "Knight", 500, 200, 20, 20, 5, 1, 1, 1, 1, UnitCategory.Infantry, 1)
    ];

    [Fact]
    public void Calculate_WithAliveAndDeadUnits_ShouldReturnCorrectBattleResult()
    {
        var calculator = new AutoBattleCalculator(_templates);
        var attackerPeasant = Unit.CreateWithCustomStatus("id_1", FactionType.England, _templates[0], 0);
        var attackerKnight = Unit.CreateWithCustomStatus("id_2", FactionType.England, _templates[1], 0);
        var defenderPeasant = Unit.CreateWithCustomStatus("id_3", FactionType.France, _templates[0], 0);
        var defenderKnight = Unit.CreateWithCustomStatus("id_4", FactionType.France, _templates[1], 0);
        defenderPeasant.TakeDamage(defenderPeasant.Health);

        var result = calculator.Calculate([attackerPeasant, attackerKnight], [defenderPeasant, defenderKnight]);

        result.IsAttackerWinner.Should().Be(true);
        result.AttackerLog.Should().HaveCount(2);
        attackerPeasant.Health.Should().Be(70);
        attackerKnight.Health.Should().Be(140);

        result.DefenderLog.Should().HaveCount(1);
        result.DefenderLog.Should().NotContain(l => l.UnitId == defenderPeasant.Id);

    }

    [Fact]
    public void Calculate_WhenTemplateNotFound_ShouldThrow()
    {
        var calculator = new AutoBattleCalculator(_templates);
        var attacker = Unit.CreateWithCustomStatus("id_1", FactionType.England, _templates[0], 0);
        var brokenTemplate = new UnitTemplate(UnitType.None, "None", 10, 10, 10, 10, 10, 1, 1, 1, 1, UnitCategory.Infantry, 1);
        var defender = Unit.CreateWithCustomStatus("id_2", FactionType.France, brokenTemplate, 0);

        var action = () => calculator.Calculate([attacker], [defender]);

        action.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void Calculate_WhenOneArmyIsEmpty_ShouldThrow()
    {
        var calculator = new AutoBattleCalculator(_templates);
        var attacker = Unit.CreateWithCustomStatus("id", FactionType.England, _templates[0], 0);

        var action = () => calculator.Calculate([attacker], []);

        action.Should().Throw<ArgumentException>();
    }
}