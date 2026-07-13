using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUnitRepository(AppDbContext dbContext) : IUnitRepository
{
    public void Add(Unit unit) => dbContext.Units.Add(unit);

    public async Task<Unit?> GetUnitAsync(UnitId id, CancellationToken cancellationToken)
    {
        return await dbContext.Units.FindAsync([id], cancellationToken);
    }
}