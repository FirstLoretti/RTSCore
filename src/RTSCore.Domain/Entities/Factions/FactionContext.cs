namespace RTSCore.Domain.Entities.Factions;

public readonly record struct FactionContext(
    FactionType FactionType,
    int Gold,
    int CitiesCount,
    int MilitaryPower
);