namespace RTSCore.Domain.ValueObjects.Configurations;

public readonly record struct FactionConfiguration(
    int InitialGold = 5000
);