using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Diplomacy.OfferResponses;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Infrastructure.Persistence;

using static Tests.TestData;

namespace Tests.Application.Campaign.Diplomacy.OfferResponses;

[Collection("TestCollection")]
public sealed class AcceptOfferHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldAcceptTradeOfferAndSaveChangesToDb_WhenAllConditionsMet()
    {
        var relation = CreateDiplomacyRelation(CampaignId.New(), FactionType.England, FactionType.France, standing: 0);
        var offer = CreateDiplomacyOffer(relation.CampaignId, FactionType.England, FactionType.France, OfferType.TradeAgreement);

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new AcceptOfferCommand(offer.Id));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbOffer = await context.DiplomacyOffers
                .AsNoTracking()
                .FirstAsync();
            dbOffer.Status.Should().Be(OfferStatus.Accepted);

            var dbRelation = await context.DiplomacyRelations
                .AsNoTracking()
                .FirstAsync();
            dbRelation.HasTradeAgreement.Should().BeTrue();
            dbRelation.Standing.Should().Be(relation.Standing + DiplomacyConfig.AcceptTradeOfferBonus);
        }
    }

    [Fact]
    public async Task Handle_ShouldAcceptPeaceOfferAndSaveChangesToDb_WhenAllConditionsMet()
    {
        var relation = CreateDiplomacyRelation(CampaignId.New(), FactionType.England, FactionType.France, standing: 0);
        var offer = CreateDiplomacyOffer(relation.CampaignId, FactionType.England, FactionType.France, OfferType.PeaceTreaty);
        relation.DeclareWar(standingPenalty: 0);

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new AcceptOfferCommand(offer.Id));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbOffer = await context.DiplomacyOffers
                .AsNoTracking()
                .FirstAsync();
            dbOffer.Status.Should().Be(OfferStatus.Accepted);

            var dbRelation = await context.DiplomacyRelations
                .AsNoTracking()
                .FirstAsync();
            dbRelation.InWar.Should().BeFalse();
            dbRelation.Standing.Should().Be(relation.Standing + DiplomacyConfig.AcceptPeaceOfferBonus);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}