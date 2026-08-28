namespace RTSCore.Domain.ValueObjects;

public record BuildingTemplate(
    BuildingType Type,
    string DisplayName,
    int Cost,
    int TurnsToConstruct
);