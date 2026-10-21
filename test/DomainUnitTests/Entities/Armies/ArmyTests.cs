using System.Numerics;

using FluentAssertions;

using Microsoft.VisualBasic;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Armies.Combat;
using RTSCore.Domain.Errors;

namespace DomainUnitTests.Entities.Armies;

public partial class ArmyTests
{
    public class Create
    {
        [Fact]
        public void ShouldCreateArmyWithGeneral()
        {
            var army = TestData.CreateArmy();

            army.GeneralId.Should().NotBe(UnitId.Empty);
            army.Units.Count.Should().Be(1);
        }
    }

    public class GetReadyCombatants
    {
        [Fact]
        public void ShouldReturnReadyToBattleUnits()
        {
            var army = TestData.CreateArmy();
            army.RecruitUnit(TestData.CreateUnitTemplate(maxHealth: 100));
            var combatantId = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 100)).Value;
            var victimId = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 100)).Value;

            army.TakeCasualties([new UnitBattleLog(victimId, DamageTaken: 100)]);

            var combatants = army.GetReadyCombatants();

            combatants.Should().HaveCount(2)
                .And.Contain(c => c.Id == army.GeneralId)
                .And.Contain(c => c.Id == combatantId);
        }
    }

    public class RestoreMovementPoints
    {
        [Fact]
        public void ShouldRestoreMaxPoints()
        {
            var army = TestData.CreateArmy();

            army.RestoreMovementPoints(TestData.MovementConfiguration);

            army.MovementPoints.Should().Be(TestData.MovementConfiguration.MaxMovementPoints);
        }
    }

    public class MoveTo
    {
        [Fact]
        public void ShouldFailWithPointsError_WhenMovementPointsNotEnough()
        {
            var army = TestData.CreateArmy();

            var result = army.MoveTo(destination: Vector2.Zero, movementCost: 1);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.NotEnoughMovementPoints);
        }

        [Fact]
        public void ShouldSuccess_WhenMovementPointsEnough()
        {
            var army = TestData.CreateArmy();

            army.RestoreMovementPoints(TestData.MovementConfiguration);
            var result = army.MoveTo(destination: Vector2.One, movementCost: 1);

            army.MovementPoints.Should().Be(TestData.MovementConfiguration.MaxMovementPoints - 1);
            army.Coordinates.Should().Be(Vector2.One);

            result.IsSuccess.Should().BeTrue();
        }
    }

    public class RecruitUnit
    {
        [Fact]
        public void ShouldAddUnitAndReturnSuccessWithId()
        {
            var army = TestData.CreateArmy();

            var result = army.RecruitUnit(TestData.CreateUnitTemplate(maxHealth: 100));

            result.Value.Should().NotBe(UnitId.Empty);
            army.Units.Count.Should().Be(2);
            army.Units.Single(u => u.Id == result.Value).IsRecruited.Should().BeFalse();
        }

        [Fact]
        public void ShouldFailWithSlotError_WhenArmyIsFull()
        {
            var army = TestData.CreateArmy();
            var template = TestData.CreateUnitTemplate(maxHealth: 100);
            foreach (var _ in Enumerable.Range(1, Army.MaxSize - 1))
            {
                army.RecruitUnit(template);
            }

            var result = army.RecruitUnit(TestData.CreateUnitTemplate(maxHealth: 100));

            result.IsFailure.Should().BeTrue();
            army.Units.Count.Should().Be(Army.MaxSize);
        }
    }

    public class AddReadyUnit
    {
        [Fact]
        public void ShouldAddUnitAndReturnSuccessWithId()
        {
            var army = TestData.CreateArmy();

            var result = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 100));

            result.Value.Should().NotBe(UnitId.Empty);
            army.Units.Count.Should().Be(2);
            army.Units.Single(u => u.Id == result.Value).IsRecruited.Should().BeTrue();
        }
    }

    public class ReplaceGeneral
    {
        [Fact]
        public void ShouldDisbandAndReplaceGeneral_WhenGeneralExist()
        {
            var army = TestData.CreateArmy();
            var generalId = army.GeneralId;

            army.ReplaceGeneral(TestData.CreateUnitTemplate(maxHealth: 100));

            army.Units.Count.Should().Be(1);
            army.GeneralId.Should().NotBe(generalId).And.NotBe(UnitId.Empty);
        }

        [Fact]
        public void ShouldAddNewGeneral_WhenGeneralIsEmpty()
        {
            var army = TestData.CreateArmy();
            army.TakeCasualties([new(army.GeneralId, army.Units.Single().Health)]);

            army.ReplaceGeneral(TestData.CreateUnitTemplate(maxHealth: 100));

            army.Units.Count.Should().Be(1);
            army.GeneralId.Should().NotBe(UnitId.Empty);
        }
    }

    public class DisbandUnit
    {
        [Fact]
        public void ShouldRemove_WhenUnitIsNotGeneral()
        {
            var army = TestData.CreateArmy();
            army.RecruitUnit(TestData.CreateUnitTemplate(maxHealth: 100));

            army.DisbandUnit(army.Units.ElementAt(1).Id);

            army.Units.Count.Should().Be(1);
        }

        [Fact]
        public void ShouldFail_WhenUnitIsGeneral()
        {
            var army = TestData.CreateArmy();

            var result = army.DisbandUnit(army.Units.Single().Id);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantDisbandGeneral);
            army.Units.Count.Should().Be(1);
        }
    }

    public class PurgeDeadUnits
    {
        [Fact]
        public void ShouldDeleteDeadUnits_WhenUnitsNotIsGeneral()
        {
            var army = TestData.CreateArmy();
            var template = TestData.CreateUnitTemplate(maxHealth: 100);
            var ids = Enumerable.Range(0, 2).Select(_ => army.AddReadyUnit(template).Value).ToArray();

            army.TakeCasualties([new(ids[0], 100), new(ids[1], 100)]);

            army.Units.Count.Should().Be(1);
            army.Units.Single().Id.Should().Be(army.GeneralId);
        }

        [Fact]
        public void ShouldDeleteDeadUnitAndRaiseEvent_WhenUnitIsGeneral()
        {
            var army = TestData.CreateArmy();

            army.TakeCasualties([new(army.GeneralId, army.Units.Single().Health)]);

            army.Units.Count.Should().Be(0);
            army.GeneralId.Should().Be(UnitId.Empty);
            army.DomainEvents.Single().Should().BeOfType<RemoveGeneralEvent>();
        }
    }

    public class TakeCasualties
    {
        [Fact]
        public void ShouldTakeDamageAndRemoveDeadUnits()
        {
            var army = TestData.CreateArmy();
            var unitId = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 100)).Value;

            army.TakeCasualties([new(unitId, 100), new(army.GeneralId, army.Units.First().Health - 1)]);

            army.Units.Should().ContainSingle();
            army.Units.Should().NotContain(u => u.Id == unitId);
        }
    }
}