using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Diplomacy.OfferResponses;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Infrastructure.Persistence;

using static Tests.TestData;

namespace Tests.Application.Campaign.Diplomacy.OfferResponses;

[Collection("TestCollection")]
public sealed class RejectOfferHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldRejectOffer_WhenOfferExist()
    {
        var offer = CreateDiplomacyOffer(offerType: OfferType.PeaceTreaty);

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.DiplomacyOffers.Add(offer);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new RejectOfferCommand(offer.Id));
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbOffer = await context.DiplomacyOffers
                .AsNoTracking()
                .FirstAsync();

            dbOffer.Status.Should().Be(OfferStatus.Rejeсted);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}