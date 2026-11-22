using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Numerics;

using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Infrastructure.Persistence;

public sealed class CityRepository(AppDbContext context) : ICityRepository
{
    public void Add(City city) => context.Cities.Add(city);
    public void AddRange(IEnumerable<City> cities) => context.Cities.AddRange(cities);

    public async Task<City?> GetAsync(CityId id, CancellationToken ct, bool isTracking = true) =>
        await context.Cities
            .WithTracking(isTracking)
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<City>> GetAsync(
        CampaignId campaignId,
        FactionType faction,
        CancellationToken ct,
        bool isTracking = true
    ) => await context.Cities
            .WithTracking(isTracking)
            .Where(c => c.CampaignId == campaignId && c.Faction == faction)
            .Include(c => c.Buildings)
            .ToListAsync(ct);
}