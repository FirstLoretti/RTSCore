namespace RTSCore.Domain.Services;

public static partial class GameBalance
{
    public static class Diplomacy
    {
        public const int InitialStanding = 0;
        public const int AcceptPeaceOfferBonus = 25;
        public const int RejectOfferPenalty = -5;

        public const int MinStandingForTrade = -80;
        public const int AcceptTradeOfferBonus = 10;
        public const int TradeIncomePerPartnerCity = 25;
        public const int CancelTradePenalty = -15;

        public const int DeclareWarPenalty = -50;
    }
}