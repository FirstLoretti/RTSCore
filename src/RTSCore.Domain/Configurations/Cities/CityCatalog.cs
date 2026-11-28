using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed class CityCatalog(IReadOnlyList<CityTemplate> templates)
{
    private readonly FrozenDictionary<CityType, CityTemplate> _cities = templates.ToFrozenDictionary(t => t.Type);

    public ImmutableArray<CityTemplate> AllTemplates => _cities.Values;
    public CityTemplate GetTemplate(CityType cityType) => _cities[cityType];
}