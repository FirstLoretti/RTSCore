using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Common;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Templates;

public record BuildingTemplate(
    BuildingType Type,
    string DisplayName,
    int Cost,
    int TurnsToConstruct,
    CityType[] AllowedCityTypes,
    BuildingCategory Category,
    int AiUtility,
    BuildingType[]? RequiredBuildings = null,
    BuildingEffect[]? Effects = null,
    Dictionary<UnitType, int>? Garrison = null
) : ICatalogOption<BuildingType>
{
    public BuildingType[] RequiredBuildings { get; init; } = RequiredBuildings ?? [];
    public BuildingEffect[] Effects { get; init; } = Effects ?? [];
}
