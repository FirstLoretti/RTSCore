using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Tests.Application.Campaign.AutoBattle;

public class AutoBattleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRequestValid_ShouldGetCalculateResultAndSaveToDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var battleService = Substitute.For<IAutoBattleService>();
        var attacker = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var defender = Army.Create(FactionType.France, Vector2.Zero, new UnitTemplate());

        unitOfWork.ArmyRepository.GetAsync(attacker.Id, Arg.Any<CancellationToken>()).Returns(attacker);
        unitOfWork.ArmyRepository.GetAsync(defender.Id, Arg.Any<CancellationToken>()).Returns(defender);

        var command = new AutoBattleCommand(attacker.Id, defender.Id);
        var handler = new AutoBattleCommandHandler(unitOfWork, battleService);

        await handler.Handle(command, CancellationToken.None);

        battleService.Received(1).StartAutoBattle(attacker, defender);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    // [Fact]
    // public async Task Handle_WhenAttackerArmyDoesNotExist_SouldThrow()
    // {
    //     var unitOfWork = Substitute.For<IUnitOfWork>();
    //     var calculator = Substitute.For<IAutoBattleCalculator>();
    //     var ct = CancellationToken.None;

    //     unitOfWork.ArmyRepository.GetAsync("invalid_id", ct).Returns((Army)null!);

    //     var command = new AutoBattleCommand("invalid_id", "valid_id");
    //     var handler = new AutoBattleCommandHandler(unitOfWork, calculator);

    //     var action = () => handler.Handle(command, ct);

    //     await action.Should().ThrowAsync<NotFoundException>();
    // }
}