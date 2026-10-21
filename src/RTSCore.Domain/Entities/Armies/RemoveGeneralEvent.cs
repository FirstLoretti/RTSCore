using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Armies;

public record RemoveGeneralEvent(CampaignId CampaignId, UnitId GeneralId) : IDomainEvent;