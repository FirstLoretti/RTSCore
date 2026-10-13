using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Enums;

namespace RTSCore.Domain.AI;

public record AiFactionContext(
    FactionType Faction,
    AiPersonality Personality,
    int CitiesCount,
    int MilitaryPower
);