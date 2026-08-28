using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUnitRepository(AppDbContext context) : IUnitRepository
{
    public void Add(Unit unit) => context.Units.Add(unit);
    public void Delete(Unit unit) => context.Units.Remove(unit);

    public async Task<Unit?> GetUnitAsync(UnitId id, CancellationToken cancellationToken)
    {
        return await context.Units.FindAsync([id], cancellationToken);
    }
}