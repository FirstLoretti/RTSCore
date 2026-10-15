using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed record CityCatalog(ImmutableArray<CityTemplate> Templates)
{
    private readonly FrozenDictionary<CityType, CityTemplate> _cities = Templates.ToFrozenDictionary(t => t.Type);

    public CityTemplate GetCity(CityType cityType) => _cities[cityType];
}