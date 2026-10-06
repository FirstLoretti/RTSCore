using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Templates;

public record CityTemplate(
    CityType Type,
    string Name,
    int MaxPopulation,
    float GrowthRate,
    float TaxRatePerCitizen,
    List<BuildingType> ConstructableBuildings,
    UnitType Governor
);