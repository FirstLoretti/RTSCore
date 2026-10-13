using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.AI.Economy;
using RTSCore.Domain.AI.Military;

namespace RTSCore.Domain.AI.Personality;

public record AiPersonality(
    AiPersonalityType Type,
    AiBudgetWeights Budget,
    AiUnitWeights Unit,
    AiBuildingWeights Building,
    AiDiplomacyWeights Diplomacy
);