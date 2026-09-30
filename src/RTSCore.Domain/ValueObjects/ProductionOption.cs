namespace RTSCore.Domain.ValueObjects;

public readonly record struct ProductionOption(
    string Name,
    int Cost,
    int TurnsToConstruct
);