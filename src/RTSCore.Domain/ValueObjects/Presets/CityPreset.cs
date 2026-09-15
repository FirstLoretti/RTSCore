namespace RTSCore.Domain.ValueObjects.Presets;

public record CityPreset(
    CityId Id,
    string DisplayName,
    CityType Type,
    int CurrentPopulation,
    BuildingType[] BuildingTypes,
    List<BuildingType>? ConstructOptions = null
);