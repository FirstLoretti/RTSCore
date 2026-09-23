using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Tests.Application.Campaing.ArmyMoving;

public class MoveArmyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldMoveArmySpendPointsAndSaveToDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var movementService = Substitute.For<ICampaignMovementService>();
        var ct = CancellationToken.None;

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var destination = new Vector2(10f, 10f);
        var armyId = army.Id;

        unitOfWork.ArmyRepository.GetAsync(armyId, ct).Returns(army);
        movementService.CalculateMovementCost(Arg.Any<Vector2>(), Arg.Any<Vector2>()).Returns(100);

        var command = new MoveArmyCommand(armyId, 10f, 10f);
        var handler = new MoveArmyCommandHandler(unitOfWork, movementService);

        var response = await handler.Handle(command, ct);

        army.Coordinates.Should().Be(destination);
        army.MovementPoints.Should().Be(0);

        response.ArmyId.Should().Be(armyId);
        response.X.Should().Be(destination.X);
        response.Y.Should().Be(destination.Y);
        response.MovementPoints.Should().Be(0);

        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }

    [Fact]
    public async Task Handle_WhenArmyNotFound_ShouldThrow()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var movementService = Substitute.For<ICampaignMovementService>();
        var ct = CancellationToken.None;

        unitOfWork.ArmyRepository.GetAsync("unknown_id", ct).Returns((Army)null!);

        var command = new MoveArmyCommand("unknown_id", 10f, 10f);
        var handler = new MoveArmyCommandHandler(unitOfWork, movementService);

        var action = () => handler.Handle(command, ct);
        await action.Should().ThrowAsync<NotFoundException>();

        await unitOfWork.DidNotReceive().SaveChangesAsync(ct);
    }

    [Fact]
    public async Task Handle_WhenNotEnoughMovementPoint_ShouldThrow()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var movementService = Substitute.For<ICampaignMovementService>();
        var ct = CancellationToken.None;

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());

        unitOfWork.ArmyRepository.GetAsync(army.Id, ct).Returns(army);
        movementService.CalculateMovementCost(Arg.Any<Vector2>(), Arg.Any<Vector2>()).Returns(200);

        var command = new MoveArmyCommand(army.Id, 10f, 10f);
        var handler = new MoveArmyCommandHandler(unitOfWork, movementService);

        var action = () => handler.Handle(command, ct);

        await action.Should().ThrowAsync<GameRuleException>();
    }
}