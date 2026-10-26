namespace RTSCore.Domain.Configurations.Diplomacy;

public sealed record DiplomacyConfiguration
{
    public required int InitialStanding { get; init; }
    public required int DeclareWarPenalty { get; init; }
    public required int AcceptPeaceOfferBonus { get; init; }
    public required int AcceptTradeOfferBonus { get; init; }
    public required int CancelTradePenalty { get; init; }
}