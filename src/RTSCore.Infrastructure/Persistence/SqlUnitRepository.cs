using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUnitRepository(AppDbContext dbContext) : IUnitRepository
{
    public void Save(Unit unit)
    {
        dbContext.Units.Add(unit);
        dbContext.SaveChanges();
    }

    public Unit? GetUnit(UnitId id) => dbContext.Units.Find(id);
}