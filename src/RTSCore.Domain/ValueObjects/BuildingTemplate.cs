namespace RTSCore.Domain.ValueObjects;

public record BuildingTemplate(
    BuildingType Type,
    string DisplayName,
    int Cost,
    int TurnsToConstruct,
    CityType[] AllowedCityTypes,
    BuildingType? RequiredPreviousTier = null,
    BuildingEffect[]? Effects = null
)
{
    public BuildingEffect[] Effects { get; init; } = Effects ?? [];
}