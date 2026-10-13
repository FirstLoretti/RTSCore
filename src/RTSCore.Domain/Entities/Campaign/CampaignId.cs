namespace RTSCore.Domain.ValueObjects.Identifiers;

public readonly record struct CampaignId(Guid Value)
{
    public static CampaignId New() => new(Guid.NewGuid());
    public static CampaignId Empty => new(Guid.Empty);
}