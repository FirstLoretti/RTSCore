namespace RTSCore.Domain.ValueObjects.AI;

public record DiplomacyWeights(
    //Declare War
    float WarTargetWeaknessWeight,
    float WarHostilityWeight,
    float WarThreshold,

    //Peace
    float PeaceDefeatWeight,
    float PeaceStandingWeight,
    float PeaсeThreshold,

    //Trade
    float TradeEconomicWeight,
    float TradeStandingWeight,
    float TradeThreshold
);