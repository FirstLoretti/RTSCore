using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Cities.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;

namespace RTSCore.Tests.UnitTests.Application;

public class RecruitUnitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldSpendGoldAndSaveToDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var cancelationToken = CancellationToken.None;

        var template = new UnitTemplate(UnitType.Peasant, "Unit", Cost: 100, 1, 1, 1, 1, 1, 1, 1, 0, UnitCategory.Infantry, 1);
        var unit = new Unit("id", FactionType.England, template);
        var army = Army.Create(FactionType.England, new(0f, 0f), 1, 20, unit);
        var preset = new CityPreset("id", "City", CityType.Village, 1, [BuildingType.ReqruitBarrack]);
        var city = new City(preset, FactionType.England, new(0f, 0f));
        var faction = new Faction(FactionType.England, 1000, PlayerType.Human);

        unitOfWork.FactionRepository.GetFactionAsync(faction.Type, cancelationToken).Returns(faction);
        unitOfWork.ArmyRepository.GetAsync(army.Id, cancelationToken).Returns(army);
        unitOfWork.CityRepository.GetCityByCoordAsync(army.Coordinates, cancelationToken).Returns(city);

        var command = new RecruitUnitCommand(army.Id, unit.Type, faction.Type);
        var handler = new RecruitUnitCommandHandler(unitOfWork, [template]);

        await handler.Handle(command, cancelationToken);

        faction.Gold.Should().Be(900);
        army.Units.Count.Should().Be(2);
        await unitOfWork.Received(1).SaveChangesAsync(cancelationToken);
    }
}