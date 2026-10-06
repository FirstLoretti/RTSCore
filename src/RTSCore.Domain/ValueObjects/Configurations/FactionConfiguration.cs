using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.ValueObjects.Configurations;

public record FactionConfiguration(
    IReadOnlyCollection<FactionTemplate> Templates
);