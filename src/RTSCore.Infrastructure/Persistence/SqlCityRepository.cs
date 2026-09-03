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

    public async Task<City?> GetCityWithBuildingsAsync(CityId id, CancellationToken cancellationToken)
    {
        return await context.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<City>> GetCitiesWithBuildingsAsync(CancellationToken cancellationToken)
    {
        return await context.Cities
            .Include(c => c.Buildings)
            .ToArrayAsync(cancellationToken);
    }

    public void AddRange(IEnumerable<City> cities)
    {
        context.Cities.AddRange(cities);
    }

    public async Task<Dictionary<FactionType, int>> GetFactionToCitiesCount(
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