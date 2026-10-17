namespace RTSCore.Domain.Entities.Campaigns;

public sealed class Campaign
{
    public CampaignId Id { get; }

    public int TurnNumber { get; private set; }

    internal Campaign(CampaignId id)
    {
        Id = id;
    }
}