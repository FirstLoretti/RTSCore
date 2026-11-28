using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Repositories;

public interface ICampaignRepository
{
    void Add(Domain.Entities.Campaigns.Campaign campaign);

    Task<Domain.Entities.Campaigns.Campaign?> GetAsync(CampaignId id, CancellationToken ct, bool isTracking = true);
}