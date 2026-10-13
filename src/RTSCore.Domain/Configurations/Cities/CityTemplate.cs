using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Configurations.Cities;

public record CityTemplate(
    CityType Type,
    string Name,
    int MaxPopulation,
    float GrowthRate,
    float TaxRatePerCitizen,
    List<BuildingType> ConstructableBuildings,
    UnitType Governor
);