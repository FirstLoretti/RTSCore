using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Tests.Application.Campaing.AutoBattle;

public class CalculateAutoBattleCommandHandlerTests
{
    [Fact]
    public async Task Hanlde_WhenRequestValid_ShouldReturnCorrectResponse()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var calculator = Substitute.For<IAutoBattleCalculator>();
        var ct = CancellationToken.None;

        var attacker = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var defender = Army.Create(FactionType.France, Vector2.Zero, new UnitTemplate());

        unitOfWork.ArmyRepository.GetAsync(attacker.Id, ct).Returns(attacker);
        unitOfWork.ArmyRepository.GetAsync(defender.Id, ct).Returns(defender);

        var fakeLog = new BattleResult(
            IsAttackerWon: true,
            AttackerBattleLogs: [new(attacker.GeneralId, DamageTaken: 0, IsAlive: true)],
            DefenderBattleLogs: [new(defender.GeneralId, DamageTaken: 1, IsAlive: false)]
        );

        calculator.Calculate(Arg.Any<IReadOnlyCollection<Unit>>(), Arg.Any<IReadOnlyCollection<Unit>>()).Returns(fakeLog);

        var command = new AutoBattleCommand(attacker.Id, defender.Id);
        var handler = new AutoBattleCommandHandler(unitOfWork, calculator);

        await handler.Handle(command, ct);

        unitOfWork.UnitRepository.Received(1).Delete(defender.Units[0]);
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