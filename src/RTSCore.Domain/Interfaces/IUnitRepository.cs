using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IUnitRepository
{
    void Add(Unit unit);
    Task<Unit?> GetUnitAsync(UnitId id, CancellationToken cancellationToken);
}