using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.Entities.Campaign.Diplomacy;

public interface IDiplomacyRelationRepository
{
    void Add(DiplomacyRelation relation);

    Task<DiplomacyRelation?> GetAsync(
        FactionType factionA,
        FactionType factionB,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<DiplomacyRelation>> GetDiplomacyRelations(
        FactionType faction,
        CancellationToken cancellationToken
    );
}