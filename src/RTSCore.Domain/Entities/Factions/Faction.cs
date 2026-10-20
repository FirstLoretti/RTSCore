using System.Numerics;

using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Entities.Factions;

public sealed class Faction : AggregateRoot
{
    public CampaignId CampaignId { get; }
    public FactionType Type { get; }

    public int Gold { get; private set; }

    public Faction(CampaignId campaignId, FactionTemplate template)
    {
        CampaignId = campaignId;
        Type = template.Type;
        Gold = template.Gold;
    }

    private Faction() { }

    public City FoundCity(CityTemplate template, Vector2 coordinates)
        => City.Create(CampaignId, template, Type, coordinates);

    public void SpendGold(int amount) => Gold = int.Max(int.MinValue, Gold - amount);

    public void EarnGold(int amount) => Gold = int.Min(Gold + amount, int.MaxValue);

    public void RefundGold(int amount) => Gold = int.Min(Gold + amount, int.MaxValue);
}