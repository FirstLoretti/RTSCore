using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed class BuildingCatalog(IReadOnlyList<BuildingTemplate> templates)
{
    private readonly FrozenDictionary<BuildingType, BuildingTemplate> _buildings = templates.ToFrozenDictionary(t => t.Type);

    public ImmutableArray<BuildingTemplate> AllTemplates => _buildings.Values;
    public BuildingTemplate GetTemplate(BuildingType buildingType) => _buildings[buildingType];
}