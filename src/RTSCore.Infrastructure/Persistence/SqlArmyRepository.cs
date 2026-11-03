using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Infrastructure.Persistence;

public sealed class SqlArmyRepository(AppDbContext context) : IArmyRepository
{
    public void Add(Army army) => context.Armies.Add(army);

    public async Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct) =>
        await context.Armies
            .Include(a => a.Units)
            .FirstOrDefaultAsync(a => a.Id == armyId, ct);

    public async Task<ReadOnlyCollection<Army>> GetAsync(FactionType faction, CancellationToken ct) =>
        (await context.Armies
            .Where(a => a.Faction == faction)
            .Include(a => a.Units)
            .ToListAsync(ct)).AsReadOnly();
}