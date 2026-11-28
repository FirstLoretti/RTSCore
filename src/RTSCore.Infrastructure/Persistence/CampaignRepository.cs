using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence;

public sealed class CampaignRepository(AppDbContext context) : ICampaignRepository
{
    public void Add(Campaign campaign) => context.Add(campaign);

    public async Task<Campaign?> GetAsync(CampaignId id, CancellationToken ct, bool isTracking = true) =>
        await context.Campaigns
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
}