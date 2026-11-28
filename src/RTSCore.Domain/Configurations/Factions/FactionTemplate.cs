using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;

namespace RTSCore.Domain.Configurations.Factions;

public sealed record FactionTemplate(
    FactionType Type,
    string DisplayName,
    int Gold,
    IReadOnlyList<CityPreset> CityPresets,
    IReadOnlyList<ArmyPreset> ArmyPresets,
    AiPersonalityType AiPersonalityType
);