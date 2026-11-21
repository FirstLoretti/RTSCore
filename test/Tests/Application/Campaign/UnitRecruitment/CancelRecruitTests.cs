using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Errors;

namespace Tests.Application.Campaign.UnitRecruitment;

public sealed class CancelRecruitTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private CancelRecruitCommandHandler CreateHandler() => new(_unitOfWork);

    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyNotFound()
    {
        var handler = CreateHandler();

        var action = () => handler.Handle(new CancelRecruitCommand(default, default), default);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUnitDoesNotExistInArmy()
    {
        var army = TestData.CreateArmy();
        var handler = CreateHandler();
        var unitId = UnitId.New();
        _unitOfWork.ArmyRepository.GetAsync(Arg.Any<ArmyId>(), Arg.Any<CancellationToken>()).Returns(army);

        var result = await handler.Handle(new CancelRecruitCommand(army.Id, unitId), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.UnitNotInTargetArmy(unitId, army.Id));
        army.Units.Should().ContainSingle();
    }
}