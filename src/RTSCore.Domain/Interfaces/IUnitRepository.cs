using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IUnitRepository
{
    public void Add(Unit unit);
    public void Save(Unit unit);
    public Unit? GetUnit(UnitId id);
}