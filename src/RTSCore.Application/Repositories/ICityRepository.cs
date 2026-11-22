using RTSCore.Domain;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Repositories;

public interface ICityRepository
{
    void Add(City city);
    void AddRange(IEnumerable<City> cities);

    Task<City?> GetAsync(CityId id, CancellationToken ct, bool isTracking = true);
    Task<IReadOnlyList<City>> GetAsync(FactionType faction, CancellationToken ct, bool isTracking = true);
}