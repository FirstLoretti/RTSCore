namespace RTSCore.Application.Repositories;

public interface ICampaignRepository
{
    void Add(Domain.Entities.Campaigns.Campaign campaign);
}