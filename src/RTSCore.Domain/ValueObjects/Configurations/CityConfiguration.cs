using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.ValueObjects.Configurations;

public record CityConfiguration(
    IReadOnlyCollection<CityTemplate> Templates
);