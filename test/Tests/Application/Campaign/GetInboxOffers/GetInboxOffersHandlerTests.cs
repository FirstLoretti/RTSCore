using FluentAssertions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Diplomacy.GetInboxOffers;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.GetInboxOffers;

[Collection("TestCollection")]
public sealed class GetInboxOffersHandlerTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldReturnIncomingPendingOffers()
    {
        var pendingOffer = TestData.CreateDiplomacyOffer(
            initiator: FactionType.England,
            target: FactionType.France,
            offerType: OfferType.TradeAgreement
        );
        var acceptedOffer = TestData.CreateDiplomacyOffer(
            campaignId: pendingOffer.CampaignId,
            initiator: pendingOffer.Initiator,
            target: pendingOffer.Target,
            offerType: OfferType.PeaceTreaty
        );
        acceptedOffer.Accept();

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.DiplomacyOffers.Add(pendingOffer);
            context.DiplomacyOffers.Add(acceptedOffer);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var offers = await mediator.Send(new GetInboxOffersQuery(pendingOffer.CampaignId, pendingOffer.Target));

            offers.Should().ContainSingle();
            offers[0].OfferType.Should().Be(OfferType.TradeAgreement);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}