namespace RTSCore.Domain.Configurations.Units;

public record UnitConfiguration(
    IReadOnlyList<int> ExpToNextLevel,
    UnitPowerWeights UnitPowerWeights
);