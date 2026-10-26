using System.Numerics;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed record CityPreset(
    CityType Type,
    string DisplayName,
    Vector2 Coordinates
);