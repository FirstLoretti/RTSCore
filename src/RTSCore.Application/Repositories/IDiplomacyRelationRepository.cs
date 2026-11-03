using System.Collections.ObjectModel;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Repositories;

public interface IDiplomacyRelationRepository
{
    void Add(DiplomacyRelation relation);

    Task<DiplomacyRelation?> GetAsync(CampaignId campaignId, FactionType faction1, FactionType faction2, CancellationToken ct);
    Task<ReadOnlyCollection<DiplomacyRelation>> GetAsync(CampaignId campaignId, FactionType faction, CancellationToken ct);
}