using Microsoft.EntityFrameworkCore;

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

    public async Task<IReadOnlyCollection<Unit>> GetUnitsAsync(FactionType faction, CancellationToken cancellationToken)
    {
        return await context.Units
            .Where(u => u.Faction == faction)
            .ToListAsync(cancellationToken);
    }
}