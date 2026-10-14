using Microsoft.EntityFrameworkCore;

using RTSCore.Domain;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Infrastructure.Persistence;

public class SqlArmyRepository(AppDbContext context) : IArmyRepository
{
    public void Add(Army army) => context.Armies.Add(army);

    public async Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct)
    {
        return await context.Armies
            .Include(a => a.Units)
            .FirstOrDefaultAsync(a => a.Id == armyId, ct);
    }

    public async Task<IReadOnlyCollection<Army>> GetFactionArmiesAsync(FactionType faction, CancellationToken ct)
    {
        return await context.Armies
            .Where(a => a.Faction == faction)
            .ToListAsync(ct);
    }
}