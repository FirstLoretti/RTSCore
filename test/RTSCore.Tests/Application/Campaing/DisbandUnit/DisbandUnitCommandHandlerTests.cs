using RTSCore.Domain.ValueObjects;
using RTSCore.Tests.Base;
using RTSCore.Application.Campaign.DisbandUnit;
using RTSCore.Domain.Entities;
using System.Numerics;
using NSubstitute;
using RTSCore.Domain.Interfaces;
using FluentAssertions;

namespace RTSCore.Tests.Application.Campaing.DisbandUnit;

public class DisbandUnitCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_WhenValidCommand_ShouldDeleteUnitFromArmyAndDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var unitRepository = Substitute.For<IUnitRepository>();
        var armyRepository = Substitute.For<IArmyRepository>();

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(new UnitTemplate());
        var unitForDisband = army.Units[1];

        unitRepository.GetUnitAsync(unitForDisband.Id, Arg.Any<CancellationToken>()).Returns(unitForDisband);
        armyRepository.GetAsync(army.Id, Arg.Any<CancellationToken>()).Returns(army);

        var command = new DisbandUnitCommand(unitForDisband.Id);
        var handler = new DisbandUnitCommandHandler(unitRepository, armyRepository, unitOfWork);

        await handler.Handle(command, CancellationToken.None);

        army.Received(1).DisbandUnit(unitForDisband);
        army.Units.Should().HaveCount(1);
        unitRepository.Received(1).Delete(unitForDisband);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}