using System.Collections.Immutable;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;

namespace RTSCore.Domain.Configurations.Factions;

public sealed record FactionTemplate(
    FactionType Type,
    string DisplayName,
    int Gold,
    ImmutableArray<CityPreset> CityPresets,
    AiPersonalityType AiPersonalityType
);