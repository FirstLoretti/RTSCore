using System.Numerics;

using FluentAssertions;

using NSubstitute;
using NSubstitute.ReceivedExtensions;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Tests.Application.Campaign.ArmyMoving;

public class MoveArmyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldСallServiceAndSaveChanges()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var movementService = Substitute.For<IArmyMovementService>();

        var army = Army.CreateWithMovementPoints(
            FactionType.England,
            Vector2.Zero,
            new UnitTemplate(),
            new ArmyConfiguration()
        );
        var destination = Vector2.One;

        unitOfWork.ArmyRepository.GetAsync(army.Id, Arg.Any<CancellationToken>()).Returns(army);

        var command = new MoveArmyCommand(army.Id, destination.X, destination.Y);
        var handler = new MoveArmyCommandHandler(unitOfWork, movementService);

        await handler.Handle(command, CancellationToken.None);

        movementService.Received(1).MoveTo(army, destination);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // [Fact]
    // public async Task Handle_WhenArmyNotFound_ShouldThrow()
    // {
    //     var unitOfWork = Substitute.For<IUnitOfWork>();
    //     var movementService = Substitute.For<ICampaignMovementService>();
    //     var ct = CancellationToken.None;

    //     unitOfWork.ArmyRepository.GetAsync("unknown_id", ct).Returns((Army)null!);

    //     var command = new MoveArmyCommand("unknown_id", 10f, 10f);
    //     var handler = new MoveArmyCommandHandler(unitOfWork, movementService);

    //     var action = () => handler.Handle(command, ct);
    //     await action.Should().ThrowAsync<NotFoundException>();

    //     await unitOfWork.DidNotReceive().SaveChangesAsync(ct);
    // }

    // [Fact]
    // public async Task Handle_WhenNotEnoughMovementPoint_ShouldThrow()
    // {
    //     var unitOfWork = Substitute.For<IUnitOfWork>();
    //     var movementService = Substitute.For<ICampaignMovementService>();
    //     var ct = CancellationToken.None;

    //     var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());

    //     unitOfWork.ArmyRepository.GetAsync(army.Id, ct).Returns(army);
    //     movementService.CalculateMovementCost(Arg.Any<Vector2>(), Arg.Any<Vector2>()).Returns(200);

    //     var command = new MoveArmyCommand(army.Id, 10f, 10f);
    //     var handler = new MoveArmyCommandHandler(unitOfWork, movementService);

    //     var action = () => handler.Handle(command, ct);

    //     await action.Should().ThrowAsync<GameRuleException>();
    // }
}