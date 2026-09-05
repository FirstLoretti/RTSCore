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
            public const float HostilityWeight = 0.5f;
            public const float WeaknessWeight = 0.5f;

            public const int TradeOfferThreshold = 50;
            public const float StandingWeight = 0.5f;
            public const float EconomicWeight = 0.5f;
            public const int ScorePerTargetCity = 10;
        }
    }
}