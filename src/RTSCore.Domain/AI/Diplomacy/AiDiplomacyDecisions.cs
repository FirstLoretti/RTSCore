using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.AI.Diplomacy;

public sealed class AiDiplomacyDecisions
{
    public readonly List<DiplomacyOffer> OffersToAccept = [];
    public readonly List<DiplomacyOffer> OffersToReject = [];
    public readonly List<FactionType> WarToDeclare = [];
    public readonly List<FactionType> TradeOfferToSend = [];
    public readonly List<FactionType> PeaceOfferToSend = [];
}