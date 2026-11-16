using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Diplomacy.TradeProposal;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.Diplomacy.TradeProposal;

[Collection("TestCollection")]
public sealed class SendTradeHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldCreateOfferAndSaveToDb()
    {
        var relation = TestData.CreateDiplomacyRelation();

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        DiplomacyOfferId offerId;
        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new SendTradeOfferCommand(relation.CampaignId, relation.FactionA, relation.FactionB));

            result.IsSuccess.Should().BeTrue();
            offerId = result.Value;
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbOffer = await context.DiplomacyOffers
                .AsNoTracking()
                .FirstAsync(o => o.Id == offerId);

            dbOffer.Type.Should().Be(OfferType.TradeAgreement);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}