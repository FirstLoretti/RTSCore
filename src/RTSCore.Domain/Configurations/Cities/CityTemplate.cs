using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed record CityTemplate(
    CityType Type,
    string DisplayName,
    int StartingPopulation,
    int PopulationForUpgrade,
    IReadOnlyList<BuildingType> ConstructableBuildings
);