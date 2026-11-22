using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Infrastructure.Persistence;

public sealed class FactionRepository(AppDbContext context) : IFactionRepository
{
    public void AddRange(IEnumerable<Faction> factions) => context.Factions.AddRange(factions);

    public async Task<Faction?> GetAsync(
        CampaignId campaignId,
        FactionType type,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.Factions
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(f => f.CampaignId == campaignId && f.Type == type, ct);
}