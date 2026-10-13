using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Entities.Campaign;

public class DiplomacyOffer
{
    public CampaignId CampaignId { get; init; }
    public DiplomacyOfferId Id { get; init; }
    public FactionType Initiator { get; init; }
    public FactionType Target { get; init; }
    public OfferType Type { get; init; }
    public OfferStatus Status { get; private set; }

    private DiplomacyOffer(
        CampaignId campaignId,
        DiplomacyOfferId offerId,
        FactionType initiator,
        FactionType target,
        OfferType type
    )
    {
        CampaignId = campaignId;
        Id = offerId;
        Initiator = initiator;
        Target = target;
        Type = type;
        Status = OfferStatus.Pending;
    }

    private DiplomacyOffer() { }

    public static ReDiplomacyOffer Create(
        CampaignId campaignId,
        FactionType initiator,
        FactionType target,
        OfferType offerType
    )
    {

    }

    public void Accept()
    {
        if (Status != OfferStatus.Pending)
        {
            throw new GameRuleException($"[{nameof(DiplomacyOffer)}] Нельзя принять предложение со статусом {Status}");
        }

        Status = OfferStatus.Accepted;
    }

    public void Reject() => Status = OfferStatus.Rejeсted;
}