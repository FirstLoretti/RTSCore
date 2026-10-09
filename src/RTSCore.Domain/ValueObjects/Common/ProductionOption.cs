using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Common;

public record ProductionOption(
    string Name,
    int Cost,
    int TurnsToConstruct,
    ProductionCategory Category
);