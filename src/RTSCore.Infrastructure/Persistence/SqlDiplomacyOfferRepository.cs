using Microsoft.EntityFrameworkCore;

using RTSCore.Domain;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence;

public class SqlDiplomacyOfferRepository(AppDbContext context) : IDiplomacyOfferRepository
{
    public void Add(DiplomacyOffer offer) => context.DiplomacyOffers.Add(offer);

    public async Task<DiplomacyOffer?> GetAsync(Guid id, CancellationToken ct)
        => await context.DiplomacyOffers.FindAsync([id], ct);


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