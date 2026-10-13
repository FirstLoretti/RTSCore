using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Common.Repositories;

public interface IUnitRepository
{
    void Add(Unit unit);
    void Delete(Unit unit);

    Task<Unit?> GetAsync(UnitId id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Unit>> GetUnitsAsync(FactionType faction, CancellationToken cancellationToken);
}