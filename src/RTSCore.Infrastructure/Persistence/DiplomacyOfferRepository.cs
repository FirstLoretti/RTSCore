using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence;

public sealed class DiplomacyOfferRepository(AppDbContext context) : IDiplomacyOfferRepository
{
    public void Add(DiplomacyOffer offer) => context.DiplomacyOffers.Add(offer);

    public void AddRange(IEnumerable<DiplomacyOffer> offers) => context.DiplomacyOffers.AddRange(offers);

    public async Task<DiplomacyOffer?> GetAsync(DiplomacyOfferId id, CancellationToken ct, bool isTracking = true) =>
        await context.DiplomacyOffers
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<DiplomacyOffer>> GetIncomingPendingOffersAsync(
        CampaignId campaignId,
        FactionType factionType,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.DiplomacyOffers
            .WithTracking(isTracking)
            .Where(o => o.CampaignId == campaignId && o.Status == OfferStatus.Pending && o.Target == factionType)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DiplomacyOffer>> GetPendingOffersAsync(
        CampaignId campaignId,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.DiplomacyOffers
            .WithTracking(isTracking)
            .Where(o => o.CampaignId == campaignId && o.Status == OfferStatus.Pending)
            .ToListAsync(ct);
}