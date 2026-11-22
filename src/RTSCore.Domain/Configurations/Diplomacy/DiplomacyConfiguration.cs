namespace RTSCore.Domain.Configurations.Diplomacy;

public sealed class DiplomacyConfiguration
{
    public required int InitialStanding { get; init; }
    public required int DeclareWarPenalty { get; init; }
    public required int AcceptPeaceOfferBonus { get; init; }
    public required int AcceptTradeOfferBonus { get; init; }
    public required int CancelTradePenalty { get; init; }
}