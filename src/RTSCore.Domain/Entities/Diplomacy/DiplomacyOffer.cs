using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Entities.Diplomacy;

public sealed class DiplomacyOffer
{
    public CampaignId CampaignId { get; init; }
    public DiplomacyOfferId Id { get; init; }
    public FactionType Initiator { get; init; }
    public FactionType Target { get; init; }
    public OfferType Type { get; init; }

    public OfferStatus Status { get; private set; }

    internal DiplomacyOffer(
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

    public Result Accept()
    {
        if (Status != OfferStatus.Pending) return GameErrors.InvalidOfferStatus;

        Status = OfferStatus.Accepted;

        return Result.Success();
    }

    public Result Reject()
    {
        if (Status != OfferStatus.Pending) return GameErrors.InvalidOfferStatus;

        Status = OfferStatus.Rejeсted;

        return Result.Success();
    }
}