using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Entities.Campaign;

public class Faction : AggregateRoot
{
    public CampaignId CampaignId { get; }

    public PlayerType Player { get; }
    public FactionType Type { get; }
    public int Gold { get; private set; }
    public bool IsEliminated { get; private set; }

    private Faction(
        CampaignId campaignId,
        PlayerType player,
        FactionTemplate template
    )
    {
        CampaignId = campaignId;
        Type = template.Type;
        Player = player;
        Gold = template.Gold;
    }

    public static Faction Create(
        CampaignId campaignId,
        FactionTemplate template,
        PlayerType player
    ) => new(campaignId, player, template);

    public void SpendGold(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (Gold < amount)
            throw new GameRuleException("В казне недостаточно средств");

        Gold -= amount;
    }

    public void EarnGold(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Gold += amount;
    }

    public void RefundGold(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Gold += amount;
    }
}