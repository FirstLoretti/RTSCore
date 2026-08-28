using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IBuildingRepository
{
    void Add(Building building);
    void Remove(Building building);
    void AddRange(IEnumerable<Building> buildings);

    Task<Building?> GetBuildingAsync(BuildingId id, CancellationToken cancellationToken);
    Task<IEnumerable<Building>> GetUnderConstructionAsync(CancellationToken cancellationToken);
}
