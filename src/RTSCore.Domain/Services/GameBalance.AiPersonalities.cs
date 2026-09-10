using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.AI;

namespace RTSCore.Domain.Services;

public static partial class GameBalance
{
    public static class AiPersonalities
    {
        public static readonly int TradeScorePerPartnerCity = 10;

        public static readonly AiPersonality Conqueror = new(
            new DiplomacyWeights(
                WarTargetWeaknessWeight: 0.5f,
                WarHostilityWeight: 0.5f,
                WarThreshold: 25f, // Power 1:1, Standing = 0

                PeaceDefeatWeight: 0.5f,
                PeaceStandingWeight: 0.5f,
                PeaсeThreshold: 25f, // Power 1:2, Standing = -100

                TradeEconomicWeight: 0.5f,
                TradeStandingWeight: 0.25f,
                TradeThreshold: 30f // 3 City, Standing = 0, NoTrade
            ),
            AiStrategicType.Aggressive
        );

        public static readonly AiPersonality Defender = new(
            new DiplomacyWeights(
                WarTargetWeaknessWeight: 0.15f,
                WarHostilityWeight: 0.25f,
                WarThreshold: 25f, // Power 1:2, Standing = 0, NoDeclare

                PeaceDefeatWeight: 0.5f,
                PeaceStandingWeight: 1f,
                PeaсeThreshold: 25f, // Power 0.75:1, Standing = -100, 

                TradeEconomicWeight: 0.5f,
                TradeStandingWeight: 0.5f,
                TradeThreshold: 30f // 1 City, Standing = 0
            ),
            AiStrategicType.Defensive
        );

        public static AiPersonality GetPersonality(FactionType faction) => faction switch
        {
            FactionType.England => Defender,
            FactionType.France => Conqueror,
            _ => throw new GameRuleException($"[{nameof(AiPersonalities)}] У фракции {faction} не настроен характер")
        };
    }
}