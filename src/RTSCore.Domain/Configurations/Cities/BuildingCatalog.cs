using System.Collections.Frozen;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public record BuildingCatalog(IReadOnlyCollection<BuildingTemplate> Templates)
{
    private readonly FrozenDictionary<BuildingType, BuildingTemplate> _buildings = Templates.ToFrozenDictionary(t => t.Type);

    public BuildingTemplate Get(BuildingType buildingType) => _buildings[buildingType];
}