using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Common.Configurations;

public record CityConfiguration(
    IReadOnlyCollection<CityTemplate> Templates
);