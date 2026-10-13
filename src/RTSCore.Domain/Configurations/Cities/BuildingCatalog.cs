using System.Collections.Frozen;

using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Configurations.Buldings;

public record BuildingCatalog(IReadOnlyCollection<BuildingTemplate> Templates)
{
    private readonly FrozenDictionary<BuildingType, BuildingTemplate> _buildings = Templates.ToFrozenDictionary(t => t.Type);

    public BuildingTemplate Get(BuildingType buildingType) => _buildings[buildingType];
}