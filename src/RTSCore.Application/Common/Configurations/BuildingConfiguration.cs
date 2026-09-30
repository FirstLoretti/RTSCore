using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Common.Configurations;

public record BuildingConfiguration(
    IReadOnlyCollection<BuildingTemplate> Templates
);