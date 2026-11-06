using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Infrastructure.Persistence;

public sealed class SqlFactionRepository(AppDbContext context) : IFactionRepository
{
    public void AddRange(IEnumerable<Faction> factions) => context.Factions.AddRange(factions);

    public async Task<Faction?> GetAsync(CampaignId campaignId, FactionType type, CancellationToken ct) =>
        await context.Factions.FindAsync([campaignId, type], ct);
}