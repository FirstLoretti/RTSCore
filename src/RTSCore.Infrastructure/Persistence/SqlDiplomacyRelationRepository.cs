using Microsoft.EntityFrameworkCore;

using RTSCore.Domain;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Campaign;

namespace RTSCore.Infrastructure.Persistence;

public class SqlDiplomacyRelationRepository(AppDbContext context) : IDiplomacyRelationRepository
{
    public void Add(DiplomacyRelation relation) => context.DiplomacyRelations.Add(relation);

    public async Task<DiplomacyRelation?> GetAsync(FactionType factionA, FactionType factionB, CancellationToken ct)
    {
        if (factionA == factionB) return null;

        var (first, second) = factionA < factionB
            ? (factionA, factionB)
            : (factionB, factionA);

        return await context.DiplomacyRelations
            .FirstOrDefaultAsync(r => r.FactionA == first && r.FactionB == second, ct);
    }

    public async Task<IReadOnlyCollection<DiplomacyRelation>> GetAsync(FactionType faction, CancellationToken ct)
    {
        return await context.DiplomacyRelations
            .Where(r => r.FactionA == faction || r.FactionB == faction)
            .ToListAsync(ct);
    }
}