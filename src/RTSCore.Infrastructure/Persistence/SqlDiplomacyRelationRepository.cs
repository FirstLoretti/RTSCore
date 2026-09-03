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

    public async Task<DiplomacyRelation?> GetRelationAsync(
        FactionType factionA,
        FactionType factionB,
        CancellationToken cancellationToken
    )
    {
        if (factionA == factionB) return null;

        var (first, second) = factionA < factionB
            ? (factionA, factionB)
            : (factionB, factionA);

        return await context.DiplomacyRelations
            .FirstOrDefaultAsync(r => r.FactionA == first && r.FactionB == second, cancellationToken);
    }

    public async Task<DiplomacyOffer?> GetDiplomacyOfferAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.DiplomacyOffers.FindAsync([id], cancellationToken);
    }
}