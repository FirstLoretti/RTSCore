namespace RTSCore.Domain.AI.ValueObjects;

public record DiplomacyWeights(
    float WarTargetWeaknessWeight,
    float WarHostilityWeight,
    float WarThreshold,

    float PeaceDefeatWeight,
    float PeaceStandingWeight,
    float PeaсeThreshold,

    float TradeEconomicWeight,
    float TradeStandingWeight,
    float TradeThreshold
);