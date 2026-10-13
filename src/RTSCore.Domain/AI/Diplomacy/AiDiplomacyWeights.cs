namespace RTSCore.Domain.AI.Diplomacy;

public record AiDiplomacyWeights(
    float WarPowerWeight,
    float WarRelationWeight,
    float WarThreshold,

    float PeaceDefeatWeight,
    float PeaceStandingWeight,
    float PeaсeThreshold,

    int TradeScorePerPartnerCity,
    float TradeEconomicWeight,
    float TradeStandingWeight,
    float TradeThreshold
);