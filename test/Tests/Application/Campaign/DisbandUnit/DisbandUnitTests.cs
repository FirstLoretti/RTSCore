using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.DisbandUnit;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Errors;

namespace Tests.Application.Campaign.DisbandUnit;

public sealed class DisbandUnitTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private DisbandUnitCommandHandler CreateHandler() => new(_unitOfWork);

    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyDoesNotExistInDb()
    {
        var handler = CreateHandler();

        var action = () => handler.Handle(new DisbandUnitCommand(default, default), default);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUnitDoesNotExistInArmy()
    {
        var army = TestData.CreateArmy();
        var handler = CreateHandler();
        _unitOfWork.ArmyRepository.GetAsync(army.Id, default).Returns(army);

        var result = await handler.Handle(new DisbandUnitCommand(army.Id, default), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.UnitNotInTargetArmy(default, army.Id));
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}