namespace RTSCore.Domain.Services;

public static partial class GameBalance
{
    public static class Diplomacy
    {
        public const int InitialStanding = 0;
        public const int MinStandingForTrade = -80;
        public const int AcceptTradeOfferBonus = 10;
        public const int AcceptPeaceOfferBonus = 25;
        public const int CancelTradePenalty = -15;
        public const int RejectOfferPenalty = -5;
        public const int DeclareWarPenalty = -50;

        public static class Ai
        {
            public const int WarDeclarationThreshold = 50;
            public const float WarHostilityWeight = 0.5f;
            public const float WarWeaknessWeight = 0.5f;

            public const int PeaceOfferThreshold = 35;
            public const float PeaceDefeatWeight = 0.7f;
            public const float PeaceStandingWeight = 0.5f;
            public const float PeaceDesperationRatioThreshold = 0.5f;

            public const int TradeOfferThreshold = 50;
            public const float TradeStandingWeight = 0.5f;
            public const float TradeEconomicWeight = 0.5f;
            public const int TradeScorePerTargetCity = 10;
        }
    }
}