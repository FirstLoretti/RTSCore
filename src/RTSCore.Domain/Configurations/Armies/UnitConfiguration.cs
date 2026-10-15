using System.Collections.Immutable;

namespace RTSCore.Domain.Configurations.Armies;

public sealed record UnitConfiguration(
    ImmutableArray<int> ExpToNextLevel,
    UnitStatsWeights UnitStatsWeights
);