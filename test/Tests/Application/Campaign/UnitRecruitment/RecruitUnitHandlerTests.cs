using System.Numerics;

using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;
using RTSCore.Domain.Errors;

using static Tests.TestData;

namespace Tests.Application.Campaign.UnitRecruitment;

public sealed class RecruitUnitHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private RecruitUnitCommandHandler CreateHandler() => new(_unitOfWork, UnitCatalog);

    [Fact]
    public async Task Handle_ShouldThrowAndNotSave_WhenArmyDoesNotExist()
    {
        var command = new RecruitUnitCommand(ArmyId.New(), CityId.New(), UnitType.Peasant);

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldThrowAndNotSave_WhenCityDoesNotExist()
    {
        var command = new RecruitUnitCommand(ArmyId.New(), CityId.New(), UnitType.Peasant);
        _unitOfWork.ArmyRepository.GetAsync(default(ArmyId), default).ReturnsForAnyArgs(CreateArmy());

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldThrowAndNotSave_WhenFactionDoesNotExist()
    {
        var command = new RecruitUnitCommand(ArmyId.New(), CityId.New(), UnitType.Peasant);
        _unitOfWork.ArmyRepository.GetAsync(default(ArmyId), default).ReturnsForAnyArgs(CreateArmy());
        _unitOfWork.CityRepository.GetAsync(default, default, default)
            .ReturnsForAnyArgs(CreateCity(template: CreateCityTemplate()));

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSave_WhenCityCannotRecruit()
    {
        var unitType = UnitType.Peasant;
        var (army, city) = SetupEntities(initialGold: UnitCatalog.GetTemplate(unitType).Cost);
        var command = new RecruitUnitCommand(army.Id, city.Id, unitType);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.CantRecruitBuildingNotConstructed);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSave_WhenArmyCannotRecruit()
    {
        var unitType = UnitType.Peasant;
        var requiredBuilding = UnitCatalog.GetTemplate(unitType).RequiredBuilding;
        var (army, city) = SetupEntities(initialGold: UnitCatalog.GetTemplate(unitType).Cost, building: requiredBuilding);
        var command = new RecruitUnitCommand(army.Id, city.Id, unitType);
        FillArmyToMaxCapacity(army);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.ArmyDoesNotHaveFreeSlots);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSave_WhenFactionCannotRecruit()
    {
        var unitType = UnitType.Peasant;
        var requiredBuilding = UnitCatalog.GetTemplate(unitType).RequiredBuilding;
        var (army, city) = SetupEntities(initialGold: 0, building: requiredBuilding);
        var command = new RecruitUnitCommand(army.Id, city.Id, unitType);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.NotEnoughGold);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    private (Army army, City city) SetupEntities(int initialGold, BuildingType? building = null)
    {
        var faction = CreateFaction(CampaignId.New(), FactionTemplate with { Gold = initialGold });
        var city = CreateCity(faction.CampaignId, CreateCityTemplate(), faction.Type);
        var army = CreateArmy(faction.CampaignId, faction.Type);

        if (building != null)
        {
            city.ConstructInstantly(CreateBuildingTemplate() with { Type = building.Value });
        }

        _unitOfWork.ArmyRepository.GetAsync(default(ArmyId), default).ReturnsForAnyArgs(army);
        _unitOfWork.CityRepository.GetAsync(default, default, default).ReturnsForAnyArgs(city);
        _unitOfWork.FactionRepository.GetAsync(default, default, default).ReturnsForAnyArgs(faction);

        return (army, city);
    }

    private static void FillArmyToMaxCapacity(Army army)
    {
        for (int i = army.Units.Count; i < Army.MaxSize; i++)
        {
            army.AddReadyUnit(CreateUnitTemplate(maxHealth: 1));
        }
    }
}