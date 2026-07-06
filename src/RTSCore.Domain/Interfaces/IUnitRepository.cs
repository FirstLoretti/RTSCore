using RTSCore.Domain.Entities;

namespace RTSCore.Domain.Interfaces;

public interface IUnitRepository
{
    public void Save(Unit unit);
}