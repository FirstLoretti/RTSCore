using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlFactionRepository(AppDbContext context) : IFactionRepository
{
    public void Add(Faction faction) => context.Factions.Add(faction);
    public void Remove(Faction faction) => context.Factions.Remove(faction);

    public async Task<Faction?> GetFactionAsync(FactionType faction, CancellationToken cancellationToken)
    {
        return await context.Factions.FindAsync([faction], cancellationToken);
    }

    public void AddRange(IEnumerable<Faction> factions)
    {
        context.Factions.AddRange(factions);
    }

    public async Task<bool> HasAnyAsync(CancellationToken cancellationToken)
    {
        return await context.Factions.AnyAsync(cancellationToken);
    }
}