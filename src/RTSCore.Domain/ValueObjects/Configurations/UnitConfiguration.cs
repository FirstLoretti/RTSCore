namespace RTSCore.Domain.ValueObjects.Configurations;

public record UnitConfiguration(
    IReadOnlyList<UnitTemplate> Templates,
    IReadOnlyList<int> ExpToNextLevel,
    PowerWeights PowerWeights
);