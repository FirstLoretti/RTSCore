using System.Numerics;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface ICityRepository
{
    void Add(City city);
    void Remove(City city);
    void AddRange(IEnumerable<City> cities);

    Task<City?> GetAsync(CityId id, CancellationToken ct);
    Task<City?> GetReadOnlyAsync(CityId id, CancellationToken ct);
    Task<City?> GetCityByCoordAsync(Vector2 coordinates, CancellationToken ct);
    Task<City?> GetWithBuildingsAsync(CityId id, CancellationToken ct);

    Task<IReadOnlyList<City>> GetWithBuildingsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<City>> GetWithBuildingsAsync(FactionType faction, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<City>> GetCitiesAsync(FactionType faction, CancellationToken cancellationToken);

    Task<Dictionary<FactionType, int>> GetFactionToCityCount(
        IEnumerable<FactionType> factions,
        CancellationToken cancellationToken
    );
}