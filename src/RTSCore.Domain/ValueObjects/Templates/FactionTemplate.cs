using RTSCore.Domain.ValueObjects.Configurations.Presets;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Templates;

public record FactionTemplate(
    FactionType Type,
    string Name,
    int Gold,
    IReadOnlyCollection<CityPreset> CityPresets
);