using RTSCore.Domain.ValueObjects;
using RTSCore.Tests.Base;
using RTSCore.Application.Campaign.DisbandUnit;
using System.Numerics;
using NSubstitute;
using FluentAssertions;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Entities.Common;

namespace RTSCore.Tests.Application.Campaign.DisbandUnit;

public class DisbandUnitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidCommand_ShouldDeleteUnitFromArmyAndDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(new UnitTemplate());
        var unit = army.Units[1];

        unitOfWork.UnitRepository.GetAsync(unit.Id, Arg.Any<CancellationToken>()).Returns(unit);
        unitOfWork.ArmyRepository.GetAsync(army.Id, Arg.Any<CancellationToken>()).Returns(army);

        var command = new DisbandUnitCommand(unit.Id);
        var handler = new DisbandUnitCommandHandler(unitOfWork);

        await handler.Handle(command, CancellationToken.None);

        Assert.DoesNotContain(unit, army.Units);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}