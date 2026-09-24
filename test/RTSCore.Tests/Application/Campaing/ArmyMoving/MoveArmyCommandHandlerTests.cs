using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Tests.Application.Campaing.ArmyMoving;

public class MoveArmyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldMoveArmySpendPointsAndSaveToDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var configuration = new ArmyConfiguration();

        var army = Army.CreateWithMovementPoints(
            FactionType.England,
            Vector2.Zero,
            new UnitTemplate(),
            configuration
        );
        var destination = new Vector2(1f, 1f);

        unitOfWork.ArmyRepository.GetAsync(army.Id, Arg.Any<CancellationToken>()).Returns(army);

        var command = new MoveArmyCommand(army.Id, destination.X, destination.Y);
        var handler = new MoveArmyCommandHandler(unitOfWork, configuration);

        var response = await handler.Handle(command, CancellationToken.None);

        army.Coordinates.Should().Be(destination);
        army.MovementPoints.Should().BeLessThan(configuration.MaxMovementPoints);

        response.ArmyId.Should().Be(army.Id);
        response.X.Should().Be(destination.X);
        response.Y.Should().Be(destination.Y);
        response.MovementPoints.Should().BeLessThan(configuration.MaxMovementPoints);

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