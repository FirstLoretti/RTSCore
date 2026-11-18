using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence;

public sealed class CampaignRepository(AppDbContext context) : ICampaignRepository
{
    public void Add(Campaign campaign) => context.Add(campaign);
}