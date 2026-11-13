using System.Numerics;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Infrastructure.Persistence;

using static Tests.TestData;

namespace Tests.Application.Campaign.UnitRecruitment;

[Collection("TestCollection")]
public sealed class RecruitUnitHandlerIntegrationTest(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldRecruitAndSaveToDb_WhenAllConditionsMet()
    {
        var unit = UnitCatalog.GetTemplate(UnitType.Peasant);
        var faction = CreateFaction(template: FactionTemplate with { Gold = unit.Cost });
        var city = City.Create(faction.CampaignId, CreateCityTemplate(), faction.Type, Vector2.Zero);
        var army = Army.Create(faction.CampaignId, faction.Type, city.Coordinates, unit);
        city.ConstructInstantly(CreateBuildingTemplate() with { Type = unit.RequiredBuilding });

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.Add(faction);
            context.Cities.Add(city);
            context.Armies.Add(army);

            await context.SaveChangesAsync();
        }

        UnitId unitId;
        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new RecruitUnitCommand(army.Id, city.Id, unit.Type));

            result.IsSuccess.Should().BeTrue();
            unitId = result.Value;
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbFaction = await context.Factions
                .AsNoTracking()
                .FirstAsync();
            dbFaction.Gold.Should().Be(faction.Gold - unit.Cost);

            var dbArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync();
            dbArmy.Units.Should().Contain(u => u.Id == unitId);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}