using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUnitRepository(AppDbContext context) : IUnitRepository
{
    public void Add(Unit unit) => context.Units.Add(unit);
    public void Delete(Unit unit) => context.Units.Remove(unit);

    public async Task<Unit?> GetAsync(UnitId id, CancellationToken cancellationToken)
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