using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

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