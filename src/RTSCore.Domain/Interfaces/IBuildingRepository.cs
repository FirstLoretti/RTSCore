using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IBuildingRepository
{
    Task<Building?> GetBuildingAsync(BuildingId id, CancellationToken cancellationToken);
}
