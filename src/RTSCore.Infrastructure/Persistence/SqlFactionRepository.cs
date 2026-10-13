using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Infrastructure.Persistence;

public class SqlFactionRepository(AppDbContext context) : IFactionRepository
{
    public void Add(Faction faction) => context.Factions.Add(faction);
    public void AddRange(IEnumerable<Faction> factions)
        => context.Factions.AddRange(factions);
    public void Remove(Faction faction) => context.Factions.Remove(faction);

    public async Task<Faction?> GetAsync(FactionType type, CancellationToken ct)
        => await context.Factions.FindAsync([type], ct);

    public async Task<IReadOnlyCollection<Faction>> GetAsync(CancellationToken ct)
        => await context.Factions.ToListAsync(ct);

    public async Task<bool> HasAnyAsync(CancellationToken ct)
        => await context.Factions.AnyAsync(ct);
}