using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence;

public sealed class ArmyRepository(AppDbContext context) : IArmyRepository
{
    public void Add(Army army) => context.Armies.Add(army);
    public void AddRange(IEnumerable<Army> armies) => context.Armies.AddRange(armies);

    public async Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct, bool isTracking = true) =>
        await context.Armies
            .WithTracking(isTracking)
            .Include(a => a.Units)
            .FirstOrDefaultAsync(a => a.Id == armyId, ct);

    public async Task<IReadOnlyList<Army>> GetAsync(
        CampaignId campaignId,
        FactionType faction,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.Armies
            .WithTracking(isTracking)
            .Where(a => a.CampaignId == campaignId && a.Faction == faction)
            .Include(a => a.Units)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Army>> GetAsync(
        CampaignId campaignId,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.Armies
            .WithTracking(isTracking)
            .Where(a => a.CampaignId == campaignId)
            .Include(a => a.Units)
            .ToListAsync(ct);

}