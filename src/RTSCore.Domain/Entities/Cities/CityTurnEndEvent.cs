using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.Entities.Cities;

public sealed record CityTurnEndEvent(
    CampaignId CampaignId,
    FactionType Faction,
    int BuildingsIncome,
    int TaxesIncome
) : IDomainEvent;