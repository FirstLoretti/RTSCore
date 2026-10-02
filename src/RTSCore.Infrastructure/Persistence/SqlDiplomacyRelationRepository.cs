using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlDiplomacyRelationRepository(AppDbContext context) : IDiplomacyRelationRepository
{
    public void Add(DiplomacyRelation relation)
    {
        context.DiplomacyRelations.Add(relation);
    }

    public async Task<DiplomacyRelation?> GetAsync(
        FactionType factionA,
        FactionType factionB,
        CancellationToken ct
    )
    {
        if (factionA == factionB) return null;

        var (first, second) = factionA < factionB
            ? (factionA, factionB)
            : (factionB, factionA);

        return await context.DiplomacyRelations
            .FirstOrDefaultAsync(r => r.FactionA == first && r.FactionB == second, ct);
    }

    public async Task<IReadOnlyList<DiplomacyRelation>> GetDiplomacyRelations(
        FactionType faction,
        CancellationToken cancellationToken
    )
    {
        return await context.DiplomacyRelations
            .AsNoTracking()
            .Where(r => r.HasTradeAgreement && !r.InWar)
            .Where(r => r.FactionA == faction || r.FactionB == faction)
            .ToArrayAsync(cancellationToken);
    }
}