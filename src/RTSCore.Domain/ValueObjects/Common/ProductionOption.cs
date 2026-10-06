namespace RTSCore.Domain.ValueObjects.Common;

public readonly record struct ProductionOption(
    string Name,
    int Cost,
    int TurnsToConstruct
);