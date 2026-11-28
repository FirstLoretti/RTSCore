using System.Runtime.CompilerServices;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.AI.Diplomacy;

public static class AiOfferEvaluator
{
    public static bool IsOfferProfitable(
        DiplomacyOffer offer,
        DiplomacyRelation relation,
        FactionContext faction,
        AiPersonality personality,
        FactionContext opponentFaction
    )
    {
        return offer.Type switch
        {
            OfferType.TradeAgreement => IsTradeOfferProfitable(relation, faction, personality, opponentFaction),
            OfferType.PeaceTreaty => IsPeaceOfferProfitable(relation, faction, personality, opponentFaction),
            _ => throw new SwitchExpressionException($"Unhandled offer type: {offer.Type}")
        };
    }

    internal static bool IsTradeOfferProfitable(
        DiplomacyRelation relation,
        FactionContext faction,
        AiPersonality personality,
        FactionContext opponentFaction
    )
    {
        if (relation.HasTradeAgreement || relation.InWar) return false;

        var score = CalculateTradeScore(relation, personality, opponentFaction);

        return score > personality.Diplomacy.TradeThreshold;
    }

    internal static bool IsPeaceOfferProfitable(
        DiplomacyRelation relation,
        FactionContext faction,
        AiPersonality personality,
        FactionContext opponentFaction
    )
    {
        if (!relation.InWar || opponentFaction.MilitaryPower == 0) return false;

        var score = CalculatePeaceScore(faction, personality, opponentFaction, relation);

        return score > personality.Diplomacy.PeaсeThreshold;
    }

    internal static bool IsDeclareWarProfitable(
        DiplomacyRelation relation,
        FactionContext faction,
        AiPersonality personality,
        FactionContext opponentFaction
    )
    {
        if (relation.InWar) return false;
        if (opponentFaction.MilitaryPower == 0) return true;

        var score = CalculateWarScore(relation, faction, personality, opponentFaction);

        return score > personality.Diplomacy.WarThreshold;
    }

    private static int CalculateWarScore(
        DiplomacyRelation relation,
        FactionContext faction,
        AiPersonality personality,
        FactionContext opponentFaction
    )
    {
        var relationScore = (DiplomacyRelation.MaxStanding - relation.Standing) * 0.5f;

        var powerRatio = (float)faction.MilitaryPower / opponentFaction.MilitaryPower;
        var powerScore = float.Clamp((powerRatio - 1f) * 100f, 0f, 100f);

        return (int)(
            (relationScore * personality.Diplomacy.WarRelationWeight) +
            (powerScore * personality.Diplomacy.WarPowerWeight)
        );
    }

    private static int CalculateTradeScore(
        DiplomacyRelation relation,
        AiPersonality personality,
        FactionContext opponent
    )
    {
        var standingScore = (int)((relation.Standing + DiplomacyRelation.MaxStanding) * 0.5f);

        var economicScore = Math.Min(
            opponent.CitiesCount * personality.Diplomacy.PartnerTradeScorePerCity,
            DiplomacyRelation.MaxStanding
        );

        return (int)(
            (standingScore * personality.Diplomacy.TradeStandingWeight) +
            (economicScore * personality.Diplomacy.TradeEconomicWeight)
        );
    }

    private static int CalculatePeaceScore(
        FactionContext faction,
        AiPersonality personality,
        FactionContext opponentFaction,
        DiplomacyRelation relation
    )
    {
        var totalPower = faction.MilitaryPower + opponentFaction.MilitaryPower;
        var powerRatio = (float)faction.MilitaryPower / totalPower;
        var powerScore = (1f - powerRatio) * 100f;

        var standingScore = (relation.Standing + 100) * 0.5f;

        return (int)(
            (powerScore * personality.Diplomacy.PeacePowerWeight) +
            (standingScore * personality.Diplomacy.PeaceStandingWeight)
        );
    }
}