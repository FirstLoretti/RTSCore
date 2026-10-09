using RTSCore.Domain.ValueObjects.Common;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Templates;

public record BuildingTemplate(
    BuildingType Type,
    string DisplayName,
    int Cost,
    int TurnsToConstruct,
    CityType[] AllowedCityTypes,
    ProductionCategory ProductionCategory,
    int AiUtility,
    BuildingType[]? RequiredBuildings = null,
    BuildingEffect[]? Effects = null,
    Dictionary<UnitType, int>? Garrison = null
)
{
    public BuildingType[] RequiredBuildings { get; init; } = RequiredBuildings ?? [];
    public BuildingEffect[] Effects { get; init; } = Effects ?? [];
}
