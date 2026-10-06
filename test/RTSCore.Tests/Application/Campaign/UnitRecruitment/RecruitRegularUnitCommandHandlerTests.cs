using System.Numerics;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Services;
using RTSCore.Domain.Services.Units;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Tests.Application.Campaign.UnitRecruitment;

public class RecruitRegularUnitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCallServiceAndSaveChanges()
    {
        var context = new RecruitmentTestContext();

        var command = new RecruitRegularUnitCommand(
            context.Army.Id,
            context.UnitTemplates.First().Type,
            context.Faction.Type
        );
        var handler = new RecruitRegularUnitCommandHandler(
            context.UnitOfWork,
            context.UnitTemplates,
            context.UnitRecruitmentService
        );

        await handler.Handle(command, CancellationToken.None);

        context.UnitRecruitmentService.Received(1)
            .RecruitUnit(context.UnitTemplates.First(), context.Army, context.Faction, context.City);
        await context.UnitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}

public sealed class RecruitmentTestContext
{
    public Army Army { get; }
    public Faction Faction { get; }
    public City City { get; }
    public UnitTemplate[] UnitTemplates = [new UnitTemplate()];

    public IUnitOfWork UnitOfWork { get; }
    public IUnitRecruitmentService UnitRecruitmentService { get; }

    public RecruitmentTestContext()
    {
        UnitOfWork = Substitute.For<IUnitOfWork>();
        UnitRecruitmentService = Substitute.For<IUnitRecruitmentService>();

        Army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        City = City.CreateEmpty(CityType.Village, Army.Coordinates, Army.Faction);
        Faction = Faction.Create(Army.Faction, PlayerType.Human, new FactionConfiguration());

        UnitOfWork.FactionRepository.GetFactionAsync(Faction.Type, Arg.Any<CancellationToken>())
            .Returns(Faction);
        UnitOfWork.ArmyRepository.GetAsync(Army.Id, Arg.Any<CancellationToken>())
            .Returns(Army);
        UnitOfWork.CityRepository.GetCityByCoordAsync(Army.Coordinates, Arg.Any<CancellationToken>())
            .Returns(City);
    }
}