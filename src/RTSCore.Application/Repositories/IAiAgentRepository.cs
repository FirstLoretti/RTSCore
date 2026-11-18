using RTSCore.Domain;
using RTSCore.Domain.AI;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Repositories;

public interface IAiAgentRepository
{
    void AddRange(IEnumerable<AiFactionAgent> aiAgents);

    Task<AiFactionAgent?> GetAsync(
        CampaignId campaignId,
        FactionType factionType,
        CancellationToken ct,
        bool isTracking = true
    );
    Task<IReadOnlyList<AiFactionAgent>> GetAsync(CampaignId campaignId, CancellationToken ct, bool isTracking = true);
}