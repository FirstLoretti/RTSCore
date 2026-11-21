using System.Numerics;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Factions;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.UnitRecruitment;

[Collection("TestCollection")]
public sealed class CancelRecruitIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldCancelAndRefundGold()
    {
        var campaignId = CampaignId.New();
        var unit = TestData.CreateUnitTemplate(maxHealth: 1);
        var faction = new Faction(campaignId, TestData.FactionTemplate);
        var army = Army.Create(campaignId, faction.Type, Vector2.Zero, unit);
        var unitId = army.RecruitUnit(unit).Value;

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            context.Factions.Add(faction);
            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new CancelRecruitCommand(army.Id, unitId));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync();
            var dbFaction = await context.Factions
                .AsNoTracking()
                .FirstAsync();

            dbArmy.Units.Should().NotContain(u => u.Id == unitId);
            dbFaction.Gold.Should().Be(faction.Gold + unit.Cost);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}