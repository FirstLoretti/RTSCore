namespace RTSCore.Domain.Entities.Cities;

public record ProductionOption(
    string Name,
    int Cost,
    int TurnsToConstruct,
    ProductionCategory Category
);