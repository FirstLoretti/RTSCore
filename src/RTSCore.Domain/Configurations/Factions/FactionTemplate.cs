using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;

namespace RTSCore.Domain.Configurations.Factions;

public record FactionTemplate(
    FactionType Type,
    string Name,
    int Gold,
    IReadOnlyCollection<CityPreset> CityPresets,
    AiPersonalityType AiPersonalityType
);