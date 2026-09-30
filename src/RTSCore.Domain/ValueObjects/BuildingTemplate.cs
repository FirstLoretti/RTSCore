using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.ValueObjects;

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
}
