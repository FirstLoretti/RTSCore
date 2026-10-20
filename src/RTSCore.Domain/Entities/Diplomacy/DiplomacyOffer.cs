using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Diplomacy;

public sealed class DiplomacyOffer
{
    public CampaignId CampaignId { get; }
    public DiplomacyOfferId Id { get; }
    public FactionType Initiator { get; }
    public FactionType Target { get; }
    public OfferType Type { get; }

    public OfferStatus Status { get; private set; }

    public DiplomacyOffer(
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
    }

    private DiplomacyOffer() { }

    public void Accept() => Status = OfferStatus.Accepted;

    public void Reject() => Status = OfferStatus.Rejeсted;
}