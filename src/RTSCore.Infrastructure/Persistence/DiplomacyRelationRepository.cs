using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence;

public sealed class DiplomacyRelationRepository(AppDbContext context) : IDiplomacyRelationRepository
{
    public void Add(DiplomacyRelation relation) => context.DiplomacyRelations.Add(relation);

    public async Task<DiplomacyRelation?> GetAsync(
        CampaignId campaignId,
        FactionType factionA,
        FactionType factionB,
        CancellationToken ct,
        bool isTracking = true
    )
    {
        var (first, second) = factionA < factionB
            ? (factionA, factionB)
            : (factionB, factionA);

        return await context.DiplomacyRelations
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(r =>
                r.CampaignId == campaignId &&
                r.FactionA == first &&
                r.FactionB == second,
                ct
        );
    }

    public async Task<IReadOnlyList<DiplomacyRelation>> GetAsync(
        CampaignId campaignId,
        FactionType faction,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.DiplomacyRelations
            .WithTracking(isTracking)
            .Where(r => r.CampaignId == campaignId && (r.FactionA == faction || r.FactionB == faction))
            .ToListAsync(ct);
}