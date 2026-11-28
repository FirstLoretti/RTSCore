using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Domain.AI;

public sealed class AiFactionAgent(CampaignId campaignId, FactionType factionType, AiPersonalityType personalityType)
{
    public CampaignId CampaignId { get; } = campaignId;
    public FactionType FactionType { get; } = factionType;

    public AiPersonalityType PersonalityType { get; private set; } = personalityType;
}