using System.Numerics;

using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlCityRepository(AppDbContext context) : ICityRepository
{
    public void Add(City city) => context.Cities.Add(city);
    public void Remove(City city) => context.Cities.Remove(city);

    public async Task<City?> GetAsync(CityId id, CancellationToken ct)
        => await context.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<City?> GetReadOnlyAsync(CityId id, CancellationToken ct)
       => await context.Cities
           .AsNoTracking()
           .Include(c => c.Buildings)
           .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<City?> GetCityByCoordAsync(Vector2 coordinates, CancellationToken cancellationToken)
    {
        return await context.Cities
            .Where(c => c.Coordinates == coordinates)
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<City?> GetWithBuildingsAsync(CityId id, CancellationToken cancellationToken)
    {
        return await context.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public void AddRange(IEnumerable<City> cities)
    {
        context.Cities.AddRange(cities);
    }

    public async Task<Dictionary<FactionType, int>> GetFactionCityCounts(
        IEnumerable<FactionType> factions,
        CancellationToken cancellationToken
    )
    {
        return await context.Cities
            .Where(c => factions.Contains(c.Faction))
            .GroupBy(c => c.Faction)
            .Select(g => new { Faction = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Faction, x => x.Count, cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetCitiesAsync(FactionType faction, CancellationToken ct)
    {
        return await context.Cities
            .Where(c => c.Faction == faction)
            .ToListAsync(ct);
    }
}