using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUnitRepository(AppDbContext dbContext) : IUnitRepository
{
    public void Add(Unit unit)
    {
        dbContext.Units.Add(unit);
    }

    public void Save(Unit unit)
    {
        dbContext.SaveChanges();
    }

    public Unit? GetUnit(UnitId id) => dbContext.Units.Find(id);
}