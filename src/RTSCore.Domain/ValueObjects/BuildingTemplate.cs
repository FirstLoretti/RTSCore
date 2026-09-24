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
    BuildingType? RequiredPreviousTier = null,
    BuildingEffect[]? Effects = null,
    Dictionary<UnitType, int>? Garrison = null
) : ICatalogOption<BuildingType>;