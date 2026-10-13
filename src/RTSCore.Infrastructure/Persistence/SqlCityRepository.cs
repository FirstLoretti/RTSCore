using System.Numerics;

using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Infrastructure.Persistence;

public class SqlCityRepository(AppDbContext context) : ICityRepository
{
    public void Add(City city) => context.Cities.Add(city);
    public void AddRange(IEnumerable<City> cities) => context.Cities.AddRange(cities);
    public void Remove(City city) => context.Cities.Remove(city);

    public async Task<City?> GetAsync(CityId id, CancellationToken ct)
        => await context.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyCollection<City>> GetAsync(FactionType factionType, CancellationToken ct)
        => await context.Cities
            .Where(c => c.Faction == factionType)
            .Include(c => c.Buildings)
            .ToListAsync(ct);

    public async Task<City?> GetReadOnlyAsync(CityId id, CancellationToken ct)
       => await context.Cities
           .AsNoTracking()
           .Include(c => c.Buildings)
           .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<City?> GetByCoordAsync(Vector2 coordinates, CancellationToken ct)
        => await context.Cities
            .Where(c => c.Coordinates == coordinates)
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(ct);
}