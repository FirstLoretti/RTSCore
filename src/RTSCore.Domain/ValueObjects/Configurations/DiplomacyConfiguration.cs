namespace RTSCore.Domain.ValueObjects.Configurations;

public record DiplomacyConfiguration(
    int InitialStanding = 0,

    int AcceptPeaceOfferBonus = 25,
    int RejectOfferPenalty = -5,

    int MinStandingForTrade = -80,
    int AcceptTradeOfferBonus = 10,
    int TradeIncomePerPartnerCity = 25,
    int CancelTradePenalty = -15,

    int DeclareWarPenalty = -50
);