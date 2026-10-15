using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed record BuildingCatalog(ImmutableArray<BuildingTemplate> Templates)
{
    private readonly FrozenDictionary<BuildingType, BuildingTemplate> _buildings = Templates.ToFrozenDictionary(t => t.Type);

    public BuildingTemplate GetBuilding(BuildingType buildingType) => _buildings[buildingType];
}