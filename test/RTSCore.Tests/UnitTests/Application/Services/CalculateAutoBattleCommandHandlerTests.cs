using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.Services;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Tests.UnitTests.Application.Services;

public class CalculateAutoBattleCommandHandlerTests
{
    [Fact]
    public async Task Hanlde_WhenRequestValid_ShouldReturnCorrectResponse()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var calculator = Substitute.For<IAutoBattleCalculator>();
        var ct = CancellationToken.None;

        var template = new UnitTemplate(UnitType.Knight, "Knight", 1, 1, 1, 1, 1, 1, 1, 1, 1, UnitCategory.Infantry, 1);
        var attackerUnit = Unit.CreateWithCustomStatus("id_1", FactionType.England, template, 0);
        var defenderUnit = Unit.CreateWithCustomStatus("id_2", FactionType.France, template, 0);
        var attacker = Army.Create(FactionType.England, new(0f, 0f), 1, 1, attackerUnit);
        var defender = Army.Create(FactionType.France, new(0f, 0f), 1, 1, defenderUnit);

        unitOfWork.ArmyRepository.GetAsync(attacker.Id, ct).Returns(attacker);
        unitOfWork.ArmyRepository.GetAsync(defender.Id, ct).Returns(defender);

        var fakeLog = new BattleResult(
            IsAttackerWon: true,
            AttackerUnitsLogs: [new(attackerUnit.Id, DamageTaken: 0, IsAlive: true)],
            DefenderUnitsLogs: [new(defenderUnit.Id, DamageTaken: 1, IsAlive: false)]
        );

        calculator.Calculate(Arg.Any<IReadOnlyCollection<Unit>>(), Arg.Any<IReadOnlyCollection<Unit>>()).Returns(fakeLog);

        var command = new AutoBattleCommand(attacker.Id, defender.Id);
        var handler = new AutoBattleCommandHandler(unitOfWork, calculator);

        await handler.Handle(command, ct);

        unitOfWork.UnitRepository.Received(1).Delete(defenderUnit);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }

    [Fact]
    public async Task Handle_WhenAttackerArmyDoesNotExist_SouldThrow()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var calculator = Substitute.For<IAutoBattleCalculator>();
        var ct = CancellationToken.None;

        unitOfWork.ArmyRepository.GetAsync("invalid_id", ct).Returns((Army)null!);

        var command = new AutoBattleCommand("invalid_id", "valid_id");
        var handler = new AutoBattleCommandHandler(unitOfWork, calculator);

        var action = () => handler.Handle(command, ct);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}