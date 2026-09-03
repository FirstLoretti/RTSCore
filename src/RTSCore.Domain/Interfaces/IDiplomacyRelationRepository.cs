using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IDiplomacyRelationRepository
{
    void Add(DiplomacyRelation relation);

    Task<DiplomacyRelation?> GetRelationAsync(
        FactionType factionA,
        FactionType factionB,
        CancellationToken cancellationToken
    );
}