using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Errors;

namespace Tests.Application.Campaign.ArmyMovement;

public sealed class MoveArmyTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private MoveArmyCommandHandler CreateHandler() => new(_unitOfWork, TestData.MovementConfiguration);

    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyNotFoundInDb()
    {
        var handler = CreateHandler();

        var action = () => handler.Handle(new MoveArmyCommand(default, default, default), default);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenArmyMovementPointsDoesNotEnough()
    {
        var army = TestData.CreateArmy();
        var handler = CreateHandler();
        _unitOfWork.ArmyRepository.GetAsync(army.Id, default).Returns(army);

        var result = await handler.Handle(new MoveArmyCommand(army.Id, 5f, 5f), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.NotEnoughMovementPoints);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}