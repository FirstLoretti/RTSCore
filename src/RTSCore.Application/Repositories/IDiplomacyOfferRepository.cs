using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Repositories;

public interface IDiplomacyOfferRepository
{
    void Add(DiplomacyOffer offer);
    void AddRange(IEnumerable<DiplomacyOffer> offers);

    Task<DiplomacyOffer?> GetAsync(DiplomacyOfferId id, CancellationToken ct, bool isTracking = true);

    Task<IReadOnlyList<DiplomacyOffer>> GetIncomingPendingOffersAsync(
        CampaignId campaignId,
        FactionType factionType,
        CancellationToken ct,
        bool isTracking = true
    );

    Task<IReadOnlyList<DiplomacyOffer>> GetPendingOffersAsync(
        CampaignId campaignId,
        CancellationToken ct,
        bool isTrackign = true
    );
}