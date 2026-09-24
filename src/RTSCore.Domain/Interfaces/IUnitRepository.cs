using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IUnitRepository
{
    void Add(Unit unit);
    void Delete(Unit unit);

    Task<Unit?> GetAsync(UnitId id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Unit>> GetUnitsAsync(FactionType faction, CancellationToken cancellationToken);
}