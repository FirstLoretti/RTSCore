using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface ICityRepository
{
    void Add(City city);
    void Remove(City city);
    void AddRange(IEnumerable<City> cities);

    Task<City?> GetCityAsync(CityId id, CancellationToken cancellationToken);
}