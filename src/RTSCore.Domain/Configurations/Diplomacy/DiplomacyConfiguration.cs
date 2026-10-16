namespace RTSCore.Domain.Configurations.Diplomacy;

public sealed record DiplomacyConfiguration
{
    public int InitialStanding { get; init; }

    public required int AcceptPeaceOfferBonus { get; init; }
    public required int RejectOfferPenalty { get; init; }

    public required int MinStandingForTrade { get; init; }
    public required int AcceptTradeOfferBonus { get; init; }
    public required int TradeIncomePerPartnerCity { get; init; }
    public required int CancelTradePenalty { get; init; }

    public required int DeclareWarPenalty { get; init; }
}