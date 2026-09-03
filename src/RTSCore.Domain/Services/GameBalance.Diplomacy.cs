namespace RTSCore.Domain.Services;

public static partial class GameBalance
{
    public static class Diplomacy
    {
        public const int StartingStanding = 0;
        public const int MinStandingForTrade = -80;
        public const int AcceptTradeOfferBonus = 10;
        public const int CancelTradePenalty = -15;
        public const int RejectOfferPenalty = -5;

        public static class Ai
        {
            public const int TradeOfferThreshold = 50;
            public const float StandingWeight = 0.5f;
            public const float EconomicWeight = 0.5f;
            public const int ScorePerTargetCity = 10;
        }
    }
}