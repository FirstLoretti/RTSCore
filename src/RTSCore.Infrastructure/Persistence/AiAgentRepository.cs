using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.AI;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence;

public sealed class AiAgentRepository(AppDbContext context) : IAiAgentRepository
{
    public void AddRange(IEnumerable<AiFactionAgent> aiAgents) => context.AiFactionAgents.AddRange(aiAgents);

    public async Task<AiFactionAgent?> GetAsync(
        CampaignId campaignId,
        FactionType factionType,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.AiFactionAgents
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(a => a.CampaignId == campaignId && a.Faction == factionType, ct);


    public async Task<IReadOnlyList<AiFactionAgent>> GetAsync(
        CampaignId campaignId,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.AiFactionAgents
            .WithTracking(isTracking)
            .Where(a => a.CampaignId == campaignId)
            .ToListAsync(ct);
}