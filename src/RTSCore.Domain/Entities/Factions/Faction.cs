using System.Numerics;

using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Errors;

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

    public Result SpendGold(int amount)
    {
        if (Gold < amount)
            return GameErrors.NotEnoughGold;

        Gold -= amount;

        return Result.Success();
    }

    public void EarnGold(int amount) => Gold += amount;

    public void RefundGold(int amount) => Gold += amount;
}