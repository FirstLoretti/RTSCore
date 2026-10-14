using System.Collections.Frozen;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public record CityCatalog(IReadOnlyCollection<CityTemplate> Templates)
{
    private readonly FrozenDictionary<CityType, CityTemplate> _cities = Templates.ToFrozenDictionary(t => t.Type);

    public CityTemplate Get(CityType cityType) => _cities[cityType];
}