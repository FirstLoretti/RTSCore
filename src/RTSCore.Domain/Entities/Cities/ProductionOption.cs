using RTSCore.Domain.Enums;

namespace RTSCore.Domain.ValueObjects;

public record ProductionOption(
    string Name,
    int Cost,
    int TurnsToConstruct,
    ProductionCategory Category
);