using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Repositories;

public interface IDiplomacyRelationRepository
{
    void Add(DiplomacyRelation relation);
    void AddRange(IEnumerable<DiplomacyRelation> relations);

    Task<DiplomacyRelation?> GetAsync(
        CampaignId campaignId,
        FactionType faction1,
        FactionType faction2,
        CancellationToken ct,
        bool isTracking = true
    );

    Task<IReadOnlyList<DiplomacyRelation>> GetAsync(
        CampaignId campaignId,
        FactionType faction,
        CancellationToken ct,
        bool isTracking = true
    );

    Task<IReadOnlyList<DiplomacyRelation>> GetAsync(
        CampaignId campaignId,
        CancellationToken ct,
        bool isTracking = true
    );
}