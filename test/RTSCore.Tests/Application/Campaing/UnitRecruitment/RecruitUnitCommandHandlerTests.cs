using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;

namespace RTSCore.Tests.Application.Campaing.UnitRecruitment;

public class RecruitUnitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldSpendGoldAndSaveToDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var cancelationToken = CancellationToken.None;

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var unit = new UnitTemplate() { Type = UnitType.Peasant };
        var preset = new CityPreset("id", "City", CityType.Village, 1, [BuildingType.ReqruitBarrack]);
        var city = new City(preset, FactionType.England, Vector2.Zero);
        var faction = new Faction(FactionType.England, 1000, PlayerType.Human);

        unitOfWork.FactionRepository.GetFactionAsync(faction.Type, cancelationToken).Returns(faction);
        unitOfWork.ArmyRepository.GetAsync(army.Id, cancelationToken).Returns(army);
        unitOfWork.CityRepository.GetCityByCoordAsync(army.Coordinates, cancelationToken).Returns(city);

        var command = new RecruitUnitCommand(army.Id, unit.Type, faction.Type);
        var handler = new RecruitUnitCommandHandler(unitOfWork, [unit]);

        await handler.Handle(command, cancelationToken);

        faction.Gold.Should().Be(faction.Gold - unit.Cost);
        faction.Received(1).SpendGold(unit.Cost);
        army.Units.Count.Should().Be(2);
        army.Received(1).RecruitUnit(unit);
        await unitOfWork.Received(1).SaveChangesAsync(cancelationToken);
    }
}