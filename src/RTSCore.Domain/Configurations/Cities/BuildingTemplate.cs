using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Cities;

public sealed record BuildingTemplate
{
    public required BuildingType Type { get; init; }
    public required string DisplayName { get; init; }
    public required int Cost { get; init; }
    public required int TurnsToConstruct { get; init; }
    public required ProductionCategory ProductionCategory { get; init; }
    public required int AiUtility { get; init; }
    public required FrozenSet<CityType> AllowedCityTypes { get; init; }

    public FrozenSet<BuildingType> RequiredBuildings { get; init; } = [];
    public FrozenSet<BuildingEffect> Effects { get; init; } = [];
    public FrozenDictionary<UnitType, int> Garrison { get; init; } = FrozenDictionary<UnitType, int>.Empty;
}