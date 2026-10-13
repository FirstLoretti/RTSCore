using System.Numerics;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Entities.Campaign;

public class Faction : AggregateRoot
{
    public CampaignId CampaignId { get; init; }
    public PlayerType PlayerType { get; init; }
    public FactionType Type { get; init; }

    public AiPersonalityType? AiPersonalityType { get; private set; }
    public int Gold { get; private set; }
    public bool IsEliminated { get; private set; }
    public int CitiesCount { get; private set; }

    private Faction(
        CampaignId campaignId,
        PlayerType player,
        FactionTemplate template,
        AiPersonalityType? aiPersonalityType
    )
    {
        CampaignId = campaignId;
        Type = template.Type;
        PlayerType = player;
        Gold = template.Gold;
        AiPersonalityType = aiPersonalityType;
    }

    private Faction() { }

    public static Faction CreateHuman(
        CampaignId campaignId,
        FactionTemplate template
    ) => new(campaignId, PlayerType.Human, template, null);

    public static Faction CreateAi(
        CampaignId campaignId,
        FactionTemplate template,
        AiPersonalityType aiPersonalityType
    ) => new(campaignId, PlayerType.Ai, template, aiPersonalityType);

    public City FoundCity(CityTemplate template, Vector2 coordinates)
    {
        CitiesCount++;
        return City.Create(CampaignId, template, Type, coordinates);
    }

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