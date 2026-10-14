namespace RTSCore.Domain.Configurations.Armies;

public record UnitConfiguration(
    IReadOnlyList<int> ExpToNextLevel,
    UnitPowerWeights UnitPowerWeights
);