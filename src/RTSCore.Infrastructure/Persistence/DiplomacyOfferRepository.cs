using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence;

public sealed class DiplomacyOfferRepository(AppDbContext context) : IDiplomacyOfferRepository
{
    public void Add(DiplomacyOffer offer) => context.DiplomacyOffers.Add(offer);

    public async Task<DiplomacyOffer?> GetAsync(DiplomacyOfferId id, CancellationToken ct, bool isTracking = true) =>
        await context.DiplomacyOffers
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
}