using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlBuildingRepository(AppDbContext context) : IBuildingRepository
{
    public void Add(Building building) => context.Buildings.Add(building);
    public void Remove(Building building) => context.Buildings.Remove(building);

    public async Task<Building?> GetBuildingAsync(BuildingId id, CancellationToken cancellationToken)
    {
        return await context.Buildings.FindAsync([id], cancellationToken);
    }

    public void AddRange(IEnumerable<Building> buildings)
    {
        context.Buildings.AddRange(buildings);
    }

    public async Task<IEnumerable<Building>> GetUnderConstructionAsync(CancellationToken cancellationToken)
    {
        return await context.Buildings.Where(b => !b.IsConstructed).ToListAsync(cancellationToken);
    }
}