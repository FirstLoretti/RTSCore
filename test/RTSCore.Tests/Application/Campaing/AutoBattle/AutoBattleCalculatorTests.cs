using System.Numerics;

using FluentAssertions;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

using Unit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Tests.Application.Campaing.AutoBattle;

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
        var attackerArmy = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var defenderArmy = Army.Create(FactionType.France, Vector2.Zero, new UnitTemplate());
        var attackerPeasant = new Unit(FactionType.England, _templates[0], attackerArmy.Id);
        var attackerKnight = new Unit(FactionType.England, _templates[1], attackerArmy.Id);
        var defenderPeasant = new Unit(FactionType.France, _templates[0], defenderArmy.Id);
        var defenderKnight = new Unit(FactionType.France, _templates[1], defenderArmy.Id);

        defenderPeasant.TakeDamage(defenderPeasant.Health);

        var result = calculator.Calculate([attackerPeasant, attackerKnight], [defenderPeasant, defenderKnight]);

        result.IsAttackerWon.Should().Be(true);
        result.AttackerBattleLogs.Should().HaveCount(3);
        attackerPeasant.Health.Should().Be(70);
        attackerKnight.Health.Should().Be(140);

        result.DefenderBattleLogs.Should().HaveCount(1);
        result.DefenderBattleLogs.Should().NotContain(l => l.UnitId == defenderPeasant.Id);
    }

    [Fact]
    public void Calculate_WhenTemplateNotFound_ShouldThrow()
    {
        var calculator = new AutoBattleCalculator(_templates);
        var attacker = new Unit(FactionType.England, _templates[0], "army_id1");
        var brokenTemplate = new UnitTemplate(
            UnitType.None, "Broken", 10, 10, 10, 10, 10, 1, 1, 1, 1, UnitCategory.Infantry, 1
        );
        var defender = new Unit(FactionType.France, brokenTemplate, "army_id2");

        var action = () => calculator.Calculate([attacker], [defender]);

        action.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void Calculate_WhenOneArmyIsEmpty_ShouldThrow()
    {
        var calculator = new AutoBattleCalculator(_templates);
        var attacker = new Unit(FactionType.England, _templates[0], "army_id");

        var action = () => calculator.Calculate([attacker], []);

        action.Should().Throw<ArgumentException>();
    }
}