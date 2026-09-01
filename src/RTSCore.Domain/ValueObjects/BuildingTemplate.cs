using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.ValueObjects;

public record BuildingTemplate(
    BuildingType Type,
    string DisplayName,
    int Cost,
    int TurnsToConstruct,
    CityType[] AllowedCityTypes,
    BuildingType? RequiredPreviousTier = null,
    BuildingEffect[]? Effects = null
) : ICatalogOption<BuildingType>
{
    public BuildingEffect[] Effects { get; init; } = Effects ?? [];
}