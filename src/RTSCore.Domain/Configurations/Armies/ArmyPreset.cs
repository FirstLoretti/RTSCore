using System.Numerics;

namespace RTSCore.Domain.Configurations.Armies;

public sealed record ArmyPreset(
    FactionType Faction,
    UnitTemplate General,
    IReadOnlyList<UnitTemplate> Units,
    Vector2 Coordinates
);