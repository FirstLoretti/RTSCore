using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlCityRepository(AppDbContext context) : ICityRepository
{
    public void Add(City city) => context.Cities.Add(city);
    public void Remove(City city) => context.Cities.Remove(city);

    public async Task<City?> GetCityAsync(CityId id, CancellationToken cancellationToken)
    {
        return await context.Cities.FindAsync([id], cancellationToken);
    }

    public async Task<City?> GetWithBuildingsAsync(CityId id, CancellationToken cancellationToken)
    {
        return await context.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetWithBuildingsAsync(CancellationToken cancellationToken)
    {
        return await context.Cities
            .Include(c => c.Buildings)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetWithBuildingsAsync(FactionType faction, CancellationToken cancellationToken)
    {
        return await context.Cities
            .Where(c => c.OwnerFaction == faction)
            .Include(c => c.Buildings)
            .ToListAsync(cancellationToken);
    }

    public void AddRange(IEnumerable<City> cities)
    {
        context.Cities.AddRange(cities);
    }

    public async Task<Dictionary<FactionType, int>> GetFactionToCityCount(
        IEnumerable<FactionType> factions,
        CancellationToken cancellationToken
    )
    {
        return await context.Cities
            .Where(c => factions.Contains(c.OwnerFaction))
            .GroupBy(c => c.OwnerFaction)
            .Select(g => new { Faction = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Faction, x => x.Count, cancellationToken);
    }
}