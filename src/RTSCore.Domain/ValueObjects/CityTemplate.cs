namespace RTSCore.Domain.ValueObjects;

public record CityTemplate(
    CityType Type,
    string DisplayName,
    int MaxPopulation,
    float GrowthRate,
    float TaxRatePerCitizen,
    List<BuildingType> AvailableBuildings
);