using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Common.Configurations;

public record UnitConfiguration(
    IReadOnlyList<UnitTemplate> Templates,
    IReadOnlyList<int> ExpToNextLevel
);