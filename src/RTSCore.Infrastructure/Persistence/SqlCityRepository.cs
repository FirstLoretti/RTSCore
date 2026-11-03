using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Numerics;

using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Infrastructure.Persistence;

public sealed class SqlCityRepository(AppDbContext context) : ICityRepository
{
    public void Add(City city) => context.Cities.Add(city);
    public void AddRange(IEnumerable<City> cities) => context.Cities.AddRange(cities);

    public async Task<City?> GetAsync(CityId id, CancellationToken ct, bool trackChanges = true) =>
        await context.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<ReadOnlyCollection<City>> GetAsync(FactionType faction, CancellationToken ct) =>
        (await context.Cities
            .Where(c => c.Faction == faction)
            .Include(c => c.Buildings)
            .ToListAsync(ct)).AsReadOnly();
}