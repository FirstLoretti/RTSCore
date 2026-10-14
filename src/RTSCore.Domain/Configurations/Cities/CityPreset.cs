using System.Numerics;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public record CityPreset(
    CityType Type,
    string Name,
    Vector2 Coordinates
);