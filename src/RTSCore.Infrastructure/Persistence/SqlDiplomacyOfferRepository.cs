using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlDiplomacyOfferRepository(AppDbContext context) : IDiplomacyOfferRepository
{
    public void Add(DiplomacyOffer offer)
    {
        context.DiplomacyOffers.Add(offer);
    }

    public async Task<DiplomacyOffer?> GetOfferAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.DiplomacyOffers.FindAsync([id], cancellationToken);
    }

    public async Task<HashSet<FactionType>> GetFactionsUnderNegotiationAsync(
        FactionType initiator,
        CancellationToken cancellationToken
    )
    {
        var pendingOffers = await context.DiplomacyOffers
            .AsNoTracking()
            .Where(o => o.Status == OfferStatus.Pending && (o.Initiator == initiator || o.Target == initiator))
            .Select(o => new { o.Initiator, o.Target })
            .ToListAsync(cancellationToken);

        return [.. pendingOffers.Select(o => o.Initiator == initiator ? o.Target : o.Initiator)];
    }

    public async Task<IReadOnlyCollection<DiplomacyOffer>> GetFactionOffersAsync(
        FactionType faction,
        CancellationToken cancellationToken)
    {
        return await context.DiplomacyOffers
            .AsNoTracking()
            .Where(o => o.Status == OfferStatus.Pending && (o.Initiator == faction || o.Target == faction))
            .ToListAsync(cancellationToken);
    }
}