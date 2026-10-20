using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.AI;

public sealed class AiFactionAgent(CampaignId campaignId, FactionType faction, AiPersonalityType personality)
{
    public CampaignId CampaignId { get; } = campaignId;
    public FactionType Faction { get; } = faction;

    public AiPersonalityType Personality { get; private set; } = personality;
}