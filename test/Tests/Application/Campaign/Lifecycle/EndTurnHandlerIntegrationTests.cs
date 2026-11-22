using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Lifecycle;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Infrastructure.Persistence;

using static Tests.TestData;

namespace Tests.Application.Campaign.Lifecycle;

[Collection("TestCollection")]
public sealed class EndTurnHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldDoTurnEndLogicAndSaveChanges()
    {
        var faction = CreateFaction();
        var city = CreateCity(campaignId: faction.CampaignId, factionType: faction.Type);
        var army = CreateArmy(campaignId: faction.CampaignId, factionType: faction.Type);
        city.ConstructInstantly(CreateBuildingTemplate() with { Effects = [new(BuildingEffectType.GoldIncome, 1f)] });
        var unitId = army.RecruitUnit(CreateUnitTemplate(maxHealth: 1) with { TurnsToRecruit = 1 }).Value;

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.Add(faction);
            context.Cities.Add(city);
            context.Armies.Add(army);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new EndTurnCommand(faction.CampaignId, faction.Type));
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbFaction = await context.Factions
                .AsNoTracking()
                .FirstAsync(f => f.Type == faction.Type);
            dbFaction.Gold.Should().BeGreaterThan(faction.Gold);

            var dbArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync(a => a.Id == army.Id);
            var unit = dbArmy.Units.First(u => u.Id == unitId);
            unit.IsRecruited.Should().BeTrue();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}