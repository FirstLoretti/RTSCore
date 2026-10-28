namespace RTSCore.Domain.AI.Diplomacy;

public sealed record AiDiplomacyWeights(
    float WarPowerWeight,
    float WarRelationWeight,
    float WarThreshold,

    float PeacePowerWeight,
    float PeaceStandingWeight,
    float PeaсeThreshold,

    int PartnerTradeScorePerCity,
    float TradeEconomicWeight,
    float TradeStandingWeight,
    float TradeThreshold
);