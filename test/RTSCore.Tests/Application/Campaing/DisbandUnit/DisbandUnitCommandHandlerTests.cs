using RTSCore.Domain.ValueObjects;
using RTSCore.Tests.Base;
using RTSCore.Application.Campaign.DisbandUnit;
using RTSCore.Domain.Entities;
using System.Numerics;
using NSubstitute;
using RTSCore.Domain.Interfaces;
using FluentAssertions;

namespace RTSCore.Tests.Application.Campaing.DisbandUnit;

public class DisbandUnitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidCommand_ShouldDeleteUnitFromArmyAndDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(new UnitTemplate());
        var unitForDisband = army.Units[1];

        unitOfWork.UnitRepository.GetAsync(unitForDisband.Id, Arg.Any<CancellationToken>())
            .Returns(unitForDisband);
        unitOfWork.ArmyRepository.GetAsync(army.Id, Arg.Any<CancellationToken>())
            .Returns(army);

        var command = new DisbandUnitCommand(unitForDisband.Id);
        var handler = new DisbandUnitCommandHandler(unitOfWork);

        await handler.Handle(command, CancellationToken.None);

        army.Units.Should().HaveCount(1);
        unitOfWork.UnitRepository.Received(1).Delete(unitForDisband);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}