using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class SqlBuildingRepository(AppDbContext context) : IBuildingRepository
{
    public async Task<Building?> GetBuildingAsync(BuildingId id, CancellationToken cancellationToken)
    {
        return await context.Buildings.FindAsync([id], cancellationToken);
    }
}