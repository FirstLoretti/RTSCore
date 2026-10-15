using RTSCore.Domain.AI.Personality;

namespace RTSCore.Domain.AI;

public readonly record struct AiFactionContext(
    FactionType FactionType,
    AiPersonality AiPersonality,
    int Gold,
    int CitiesCount,
    int MilitaryPower
);