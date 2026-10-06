using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.ValueObjects.Configurations;

public record BuildingConfiguration(
    IReadOnlyList<BuildingTemplate> Templates
);