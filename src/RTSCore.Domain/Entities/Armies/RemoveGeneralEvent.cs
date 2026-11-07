using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Armies;

public sealed record RemoveGeneralEvent(CampaignId CampaignId, UnitId GeneralId) : IDomainEvent;