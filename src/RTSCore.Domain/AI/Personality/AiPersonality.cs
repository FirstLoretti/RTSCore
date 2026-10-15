using RTSCore.Domain.AI.Construction;
using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.AI.Recruitment;

namespace RTSCore.Domain.AI.Personality;

public sealed record AiPersonality(
    AiPersonalityType Type,
    AiBudgetWeights Budget,
    AiUnitWeights Unit,
    AiBuildingWeights Building,
    AiDiplomacyWeights Diplomacy
);