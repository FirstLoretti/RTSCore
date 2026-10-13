using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Configurations.Buldings;

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
