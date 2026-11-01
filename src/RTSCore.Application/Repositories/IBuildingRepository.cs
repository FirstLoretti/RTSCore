using RTSCore.Domain;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Repositories;

public interface IBuildingRepository
{
    void Add(Building building);
    void Remove(Building building);
    void AddRange(IEnumerable<Building> buildings);

    Task<Building?> GetBuildingAsync(BuildingId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Building>> GetUnderConstructionAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Building>> GetUnderConstructionAsync(FactionType faction, CancellationToken cancellationToken);
}
