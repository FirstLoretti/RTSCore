using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Cities;

public sealed record ConstructionCanceledEvent(
    CampaignId CampaignId,
    FactionType Faction,
    int Cost
) : IDomainEvent;