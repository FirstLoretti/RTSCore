using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Entities.Campaign;

public interface IBuildingRepository
{
    void Add(Building building);
    void Remove(Building building);
    void AddRange(IEnumerable<Building> buildings);

    Task<Building?> GetBuildingAsync(BuildingId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Building>> GetUnderConstructionAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Building>> GetUnderConstructionAsync(FactionType faction, CancellationToken cancellationToken);
}
