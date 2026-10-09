using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Presets;

namespace RTSCore.Domain.ValueObjects.Templates;

public record FactionTemplate(
    FactionType Type,
    string Name,
    int Gold,
    IReadOnlyCollection<CityPreset> CityPresets
);