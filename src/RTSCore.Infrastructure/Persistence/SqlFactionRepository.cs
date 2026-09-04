using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
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

    public async Task<IReadOnlyCollection<FactionType>> GetAnotherFactionsAsync(
        FactionType currentFaction,
        CancellationToken cancellationToken
    )
    {
        return await context.Factions
            .Where(f => f.Type != currentFaction && !f.IsEliminated)
            .Select(f => f.Type)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<FactionType, int>> GetFactionToMilitaryPower(
        IEnumerable<FactionType> factions, CancellationToken cancellationToken
    )
    {
        var militaryPower = await context.Units
            .AsNoTracking()
            .Where(u => u.TurnsToRecruit <= 0 && u.Health > 0)
            .GroupBy(u => u.OwnerFaction)
            .Select(g => new
            {
                Faction = g.Key,
                Power = (int)g.Sum(u =>
                            (u.Health * GameBalance.Units.HealthWeight) +
                            (u.Damage * GameBalance.Units.DamageWeight) +
                            (u.Armor * GameBalance.Units.ArmorWeight))
            })
            .ToListAsync(cancellationToken);

        return militaryPower.ToDictionary(l => l.Faction, l => l.Power);
    }
}