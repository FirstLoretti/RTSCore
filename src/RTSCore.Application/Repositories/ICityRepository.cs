using System.Numerics;

using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Common.Repositories;

public interface ICityRepository
{
    void Add(City city);
    void Remove(City city);
    void AddRange(IEnumerable<City> cities);

    Task<City?> GetAsync(CityId id, CancellationToken ct);
    Task<IReadOnlyCollection<City>> GetAsync(FactionType faction, CancellationToken ct);
    Task<City?> GetReadOnlyAsync(CityId id, CancellationToken ct);

    Task<City?> GetByCoordAsync(Vector2 coordinates, CancellationToken ct);
}