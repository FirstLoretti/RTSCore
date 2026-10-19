using System.Collections.Immutable;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed record CityTemplate(
    CityType Type,
    string DisplayName,
    int PopulationToUpgrade,
    ImmutableArray<BuildingType> ConstructableBuildings
);