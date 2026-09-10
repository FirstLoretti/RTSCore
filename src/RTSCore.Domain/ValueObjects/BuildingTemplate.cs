using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.ValueObjects;

public record BuildingTemplate(
    BuildingType Type,
    string DisplayName,
    int Cost,
    int TurnsToConstruct,
    CityType[] AllowedCityTypes,
    BuildingType? RequiredPreviousTier = null,
    BuildingEffect[]? Effects = null,
    Dictionary<UnitType, int>? Garrison = null
) : ICatalogOption<BuildingType>
{
    public BuildingEffect[] Effects { get; init; } = Effects ?? [];
    public Dictionary<UnitType, int> Garrison { get; init; } = Garrison ?? [];
}