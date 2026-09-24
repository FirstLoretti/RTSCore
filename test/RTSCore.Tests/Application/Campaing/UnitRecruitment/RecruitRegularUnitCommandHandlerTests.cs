using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Tests.Application.Campaing.UnitRecruitment;

public class RecruitRegularUnitCommandHandlerTests
{
    private readonly UnitTemplate[] _units = [
        new UnitTemplate() { Type = UnitType.Peasant, RequiredBuilding = BuildingType.ReqruitBarrack }
    ];

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldRecruitRegularUnitSpendGoldAndSaveToDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var factionConfiguraion = new FactionConfiguration();

        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var city = City.CreateEmpty(CityType.Village, army.Coordinates, army.Faction);
        var faction = Faction.Create(army.Faction, PlayerType.Human, factionConfiguraion);
        var building = Building.CreateConstructed(_units[0].RequiredBuilding!.Value, faction.Type, city.Id);

        city.RegisterBuilding(building);

        unitOfWork.FactionRepository.GetFactionAsync(faction.Type, Arg.Any<CancellationToken>())
            .Returns(faction);
        unitOfWork.ArmyRepository.GetAsync(army.Id, Arg.Any<CancellationToken>())
            .Returns(army);
        unitOfWork.CityRepository.GetCityByCoordAsync(army.Coordinates, Arg.Any<CancellationToken>())
            .Returns(city);

        var command = new RecruitRegularUnitCommand(army.Id, _units[0].Type, faction.Type);
        var handler = new RecruitRegularUnitCommandHandler(unitOfWork, _units, new UnitRecruitmentService());

        await handler.Handle(command, CancellationToken.None);

        faction.Gold.Should().BeLessThan(factionConfiguraion.InitialGold);
        army.Units.Count.Should().Be(2);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}