namespace RTSCore.Domain.ValueObjects.Configurations;

public record BuildingConfiguration(
    IReadOnlyList<BuildingTemplate> Buildings
);