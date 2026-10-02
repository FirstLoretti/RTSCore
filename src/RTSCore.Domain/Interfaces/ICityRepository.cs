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

    Task<IReadOnlyList<City>> GetCitiesAsync(FactionType faction, CancellationToken ct);

    Task<Dictionary<FactionType, int>> GetFactionCityCounts(
        IEnumerable<FactionType> factions,
        CancellationToken ct
    );
}