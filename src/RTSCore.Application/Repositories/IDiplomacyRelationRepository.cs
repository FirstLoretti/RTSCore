using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Common.Repositories;

public interface IDiplomacyRelationRepository
{
    void Add(DiplomacyRelation relation);

    Task<DiplomacyRelation?> GetAsync(FactionType faction1, FactionType faction2, CancellationToken ct);

    Task<IReadOnlyCollection<DiplomacyRelation>> GetAsync(FactionType faction, CancellationToken ct);
}