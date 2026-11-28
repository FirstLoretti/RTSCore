using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Lifecycle;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.Lifecycle;

[Collection("TestCollection")]
public sealed class StartCampaignHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldInitializeWorldAndSaveChanges()
    {
        HashSet<FactionType> playerFactions = [FactionType.England, FactionType.France];
        CampaignId campaignId;

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            campaignId = await mediator.Send(new StartCampaignCommand(playerFactions));
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var factions = await context.Factions
                .AsNoTracking()
                .Where(f => f.CampaignId == campaignId)
                .ToListAsync();
            factions.Should().HaveCount(TestData.FactionCatalog.AllTemplates.Length);

            var aiAgents = await context.AiFactionAgents
                .AsNoTracking()
                .Where(f => f.CampaignId == campaignId)
                .ToListAsync();
            aiAgents.Should().HaveCount(TestData.FactionCatalog.AllTemplates.Length - playerFactions.Count);
            aiAgents.Select(a => a.FactionType).Should().NotIntersectWith(playerFactions);

            var cities = await context.Cities
                .AsNoTracking()
                .Where(f => f.CampaignId == campaignId)
                .ToListAsync();
            cities.Should().HaveCount(TestData.FactionCatalog.AllTemplates.Sum(f => f.CityPresets.Count));

            await context.Campaigns
                .AsNoTracking()
                .FirstAsync(c => c.Id == campaignId);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}