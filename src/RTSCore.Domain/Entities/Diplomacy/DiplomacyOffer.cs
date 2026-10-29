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
        FactionType initiator,
        FactionType target,
        OfferType type
    )
    {
        CampaignId = campaignId;
        Id = DiplomacyOfferId.New();
        Initiator = initiator;
        Target = target;
        Type = type;
    }

    private DiplomacyOffer() { }

    public void Accept()
    {
        ThrowIfInvalidStatus();

        Status = OfferStatus.Accepted;
    }

    public void Reject()
    {
        ThrowIfInvalidStatus();

        Status = OfferStatus.Rejeсted;
    }

    private void ThrowIfInvalidStatus()
    {
        if (Status != OfferStatus.Pending)
            throw new InvalidOperationException($"Diplomacy offer:{Id} with status:{Status} cannot be accept or reject");
    }
}