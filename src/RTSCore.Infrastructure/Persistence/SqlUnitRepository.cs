using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUnitRepository(AppDbContext dbContext) : IUnitRepository
{
    public void Save(Unit unit)
    {
        dbContext.Units.Add(unit);
        dbContext.SaveChanges();
    }
}