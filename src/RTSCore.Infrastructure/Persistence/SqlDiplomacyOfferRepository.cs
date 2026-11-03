using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence;

public sealed class SqlDiplomacyOfferRepository(AppDbContext context) : IDiplomacyOfferRepository
{
    public void Add(DiplomacyOffer offer) => context.DiplomacyOffers.Add(offer);

    public async Task<DiplomacyOffer?> GetAsync(DiplomacyOfferId id, CancellationToken ct) =>
        await context.DiplomacyOffers.FindAsync([id], ct);
}