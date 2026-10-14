using RTSCore.Domain.AI.Personality;

namespace RTSCore.Domain.AI;

public record AiFactionContext(
    FactionType FactionType,
    AiPersonality AiPersonality,
    int Gold,
    int CitiesCount,
    int MilitaryPower
);