using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Armies;

public sealed record CancelRecruitmentEvent(
    CampaignId CampaignId,
    FactionType Faction,
    int Cost
) : IDomainEvent;