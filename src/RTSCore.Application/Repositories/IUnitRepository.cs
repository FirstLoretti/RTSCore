using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Domain.Common.Repositories;

public interface IUnitRepository
{
    void Add(Unit unit);
    void Delete(Unit unit);

    Task<Unit?> GetAsync(UnitId id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Unit>> GetUnitsAsync(FactionType faction, CancellationToken cancellationToken);
}