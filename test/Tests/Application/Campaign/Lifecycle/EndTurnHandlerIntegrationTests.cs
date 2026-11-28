using System.Numerics;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Lifecycle;
using RTSCore.Domain;
using RTSCore.Domain.AI;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Campaigns;
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
        var playerFaction = CreateFaction(CampaignId.New(), template: FactionTemplate with { Type = FactionType.England });
        var aiFaction = CreateFaction(playerFaction.CampaignId, template: FactionTemplate with { Type = FactionType.France });
        var playerCity = CreateCity(playerFaction.CampaignId, factionType: playerFaction.Type);
        var playerArmy = CreateArmy(playerFaction.CampaignId, factionType: playerFaction.Type);
        var aiCity = CreateCity(playerFaction.CampaignId, factionType: aiFaction.Type, coordinates: Vector2.One);
        var aiArmy = CreateArmy(playerFaction.CampaignId, factionType: aiFaction.Type, coordinates: Vector2.One);
        var ai = new AiFactionAgent(playerFaction.CampaignId, aiFaction.Type, AiPersonalityType.Conqueror);
        aiCity.ConstructInstantly(CreateBuildingTemplate() with { Type = BuildingType.ReqruitBarrack });
        playerCity.ConstructInstantly(CreateBuildingTemplate() with { Effects = [new(BuildingEffectType.GoldIncome, 1f)] });
        var unitId = playerArmy.RecruitUnit(CreateUnitTemplate(maxHealth: 1) with { TurnsToRecruit = 1 }).Value;
        var relation = CreateDiplomacyRelation(playerFaction.CampaignId, FactionType.England, FactionType.France, standing: 0);
        var campaign = new RTSCore.Domain.Entities.Campaigns.Campaign(
            playerFaction.CampaignId,
            [playerFaction.Type, aiFaction.Type],
            [playerFaction.Type]
        );

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.AddRange([playerFaction, aiFaction]);
            context.Cities.AddRange([playerCity, aiCity]);
            context.Armies.AddRange([playerArmy, aiArmy]);
            context.AiFactionAgents.Add(ai);
            context.DiplomacyRelations.Add(relation);
            context.Campaigns.Add(campaign);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new EndTurnCommand(playerFaction.CampaignId, playerFaction.Type));
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbPlayerFaction = await context.Factions
                .AsNoTracking()
                .FirstAsync(f => f.Type == playerFaction.Type);
            dbPlayerFaction.Gold.Should().BeGreaterThan(playerFaction.Gold);

            var dbPlayerArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync(a => a.Id == playerArmy.Id);
            var unit = dbPlayerArmy.Units.First(u => u.Id == unitId);
            unit.IsRecruited.Should().BeTrue();

            var dbAiFaction = await context.Factions
                .AsNoTracking()
                .FirstAsync(f => f.Type == aiFaction.Type);
            dbAiFaction.Gold.Should().BeLessThan(aiFaction.Gold);

            var dbAiArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync(a => a.Id == aiArmy.Id);
            dbAiArmy.Units.Count.Should().BeGreaterThan(1);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}