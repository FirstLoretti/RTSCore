namespace RTSCore.Domain.ValueObjects;

public record CityTemplate(
    string DisplayName,
    CityType Type,
    int MaxPopulation
);