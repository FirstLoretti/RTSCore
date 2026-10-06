using System.Numerics;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Configurations.Presets;

public record CityPreset(
    CityType CityType,
    string Name,
    Vector2 Coordinates
);