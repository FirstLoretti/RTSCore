using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlArmyRepository(AppDbContext context) : IArmyRepository
{
    public void Add(Army army) => context.Armies.Add(army);

    public async Task<Army?> GetAsync(string armyId, CancellationToken cancellationToken)
    {
        return await context.Armies
            .Include(a => a.Units)
            .FirstOrDefaultAsync(a => a.Id == armyId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Army>> GetArmiesAsync(FactionType faction, CancellationToken cancellationToken)
    {
        return await context.Armies
            .Where(a => a.Faction == faction)
            .ToArrayAsync(cancellationToken);
    }
}