namespace RTSCore.Domain.Configurations.Cities;

public sealed record CityConfiguration(
    float GrowthRate,
    float TaxPerCitizen
);