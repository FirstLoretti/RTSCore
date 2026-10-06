using System.Numerics;

using FluentAssertions;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

using Unit = RTSCore.Domain.Entities.Campaign.Unit;

namespace RTSCore.Tests.Application.Campaign.AutoBattle;

// public class AutoBattleCalculatorTests
// {
//     private readonly UnitTemplate _weak = new() { Type = UnitType.Peasant, MaxHealth = 50, TurnsToRecruit = 0 };
//     private readonly UnitTemplate _strong = new() { Type = UnitType.Militia, MaxHealth = 100, TurnsToRecruit = 0 };
//     private readonly AutoBattleConfiguration _configuration = new(
//         WinnerCasualties: 0.3f,
//         LoserCasualties: 1f,
//         DrawCasualties: 0.5f
//     );

//     [Fact]
//     public void Calculate_WhenAttackerIsStronger_ShouldReturnAttackerVictory()
//     {
//         var (attacker, defender) = CreateArmy(_strong, _weak);

//         var result = CreateCalculator.Calculate(attacker.Units, defender.Units);

//         result.IsAttackerWon.Should().Be(true);
//         result.AttackerBattleLogs.Should().ContainSingle()
//             .Which.RemainingHealth.Should().Be(70);

//         result.IsDefenderWon.Should().Be(false);
//         result.DefenderBattleLogs.Should().ContainSingle()
//             .Which.RemainingHealth.Should().Be(0);
//     }

//     [Fact]
//     public void Calculate_WhenDefenderIsStronger_ShouldReturnDefenderVictory()
//     {
//         var (attacker, defender) = CreateArmy(_weak, _strong);

//         var result = CreateCalculator.Calculate(attacker.Units, defender.Units);

//         result.IsAttackerWon.Should().Be(false);
//         result.AttackerBattleLogs.Should().ContainSingle()
//             .Which.RemainingHealth.Should().Be(0);

//         result.IsDefenderWon.Should().Be(true);
//         result.DefenderBattleLogs.Should().ContainSingle()
//             .Which.RemainingHealth.Should().Be(70);
//     }

//     [Fact]
//     public void Calculate_WhenPowersAreEqual_ShouldReturnDraw()
//     {
//         var (attacker, defender) = CreateArmy(_weak, _weak);

//         var result = CreateCalculator.Calculate(attacker.Units, defender.Units);

//         result.IsAttackerWon.Should().Be(false);
//         result.AttackerBattleLogs.Should().ContainSingle()
//             .Which.RemainingHealth.Should().Be(50);

//         result.IsDefenderWon.Should().Be(false);
//         result.DefenderBattleLogs.Should().ContainSingle()
//             .Which.RemainingHealth.Should().Be(50);
//     }

//     private AutoBattleCalculator CreateCalculator => new([_weak, _strong], _configuration);

//     private static (Army attacker, Army defender) CreateArmy(UnitTemplate attacker, UnitTemplate defender)
//     {
//         var attackerArmy = Army.Create(FactionType.England, Vector2.Zero, attacker);
//         var defenderArmy = Army.Create(FactionType.France, Vector2.Zero, defender);
//         return (attackerArmy, defenderArmy);
//     }

// [Fact]
// public void Calculate_WhenTemplateNotFound_ShouldThrow()
// {
//     var calculator = new AutoBattleCalculator(_templates);
//     var attacker = new Unit(FactionType.England, _templates[0], "army_id1");
//     var brokenTemplate = new UnitTemplate(
//         UnitType.None, "Broken", 10, 10, 10, 10, 10, 1, 1, 1, 1, UnitCategory.Infantry, 1
//     );
//     var defender = new Unit(FactionType.France, brokenTemplate, "army_id2");

//     var action = () => calculator.Calculate([attacker], [defender]);

//     action.Should().Throw<NotFoundException>();
// }

// [Fact]
// public void Calculate_WhenOneArmyIsEmpty_ShouldThrow()
// {
//     var calculator = new AutoBattleCalculator(_templates);
//     var attacker = new Unit(FactionType.England, _templates[0], "army_id");

//     var action = () => calculator.Calculate([attacker], []);

//     action.Should().Throw<ArgumentException>();
// }
//}