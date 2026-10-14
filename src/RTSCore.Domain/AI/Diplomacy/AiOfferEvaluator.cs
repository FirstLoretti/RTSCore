using System.Runtime.CompilerServices;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.AI.Diplomacy;

public static class AiOfferEvaluator
{
    public static bool IsOfferProfitable(
        DiplomacyOffer offer,
        DiplomacyRelation relation,
        AiFactionContext faction,
        AiFactionContext opponentFaction
    )
    {
        return offer.Type switch
        {
            OfferType.TradeAgreement => IsTradeOfferProfitable(relation, faction, opponentFaction),
            OfferType.PeaceTreaty => IsPeaceOfferProfitable(relation, faction, opponentFaction),
            _ => throw new SwitchExpressionException($"Unhandled offer type: {offer.Type}")
        };
    }

    internal static bool IsTradeOfferProfitable(
        DiplomacyRelation relation,
        AiFactionContext faction,
        AiFactionContext opponentFaction
    )
    {
        if (relation.HasTradeAgreement || relation.InWar) return false;
        if (relation.Standing < faction.AiPersonality.Diplomacy.TradeThreshold) return false;

        var score = CalculateTradeScore(relation, faction, opponentFaction);

        return score > faction.AiPersonality.Diplomacy.TradeThreshold;
    }

    internal static bool IsPeaceOfferProfitable(
        DiplomacyRelation relation,
        AiFactionContext faction,
        AiFactionContext opponentFaction
    )
    {
        if (!relation.InWar) return false;
        if (faction.MilitaryPower <= 0) return true;

        var score = CalculatePeaceScore(faction, opponentFaction, relation);

        return score > faction.AiPersonality.Diplomacy.PeaсeThreshold;
    }

    internal static bool IsDeclareWarProfitable(
        DiplomacyRelation relation,
        AiFactionContext faction,
        AiFactionContext opponentFaction
    )
    {
        if (relation.InWar || faction.MilitaryPower <= 0) return false;
        if (opponentFaction.MilitaryPower <= 0) return true;

        var score = CalculateWarScore(relation, faction, opponentFaction);

        return score > faction.AiPersonality.Diplomacy.WarThreshold;
    }

    private static int CalculateWarScore(
        DiplomacyRelation relation,
        AiFactionContext faction,
        AiFactionContext opponentFaction
    )
    {
        var relationScore = (DiplomacyRelation.MaxStanding - relation.Standing) * 0.5f;

        var powerRatio = (float)faction.MilitaryPower / opponentFaction.MilitaryPower;
        var powerScore = float.Clamp((powerRatio - 1f) * 100f, 0f, 100f);

        return (int)(
            (relationScore * faction.AiPersonality.Diplomacy.WarRelationWeight) +
            (powerScore * faction.AiPersonality.Diplomacy.WarPowerWeight)
        );
    }

    private static int CalculateTradeScore(
        DiplomacyRelation relation,
        AiFactionContext faction,
        AiFactionContext opponent
    )
    {
        var standingScore = (int)((relation.Standing + DiplomacyRelation.MaxStanding) * 0.5f);

        var economicScore = Math.Min(
            opponent.CitiesCount * faction.AiPersonality.Diplomacy.TradeScorePerPartnerCity,
            DiplomacyRelation.MaxStanding
        );

        return (int)(
            (standingScore * faction.AiPersonality.Diplomacy.TradeStandingWeight) +
            (economicScore * faction.AiPersonality.Diplomacy.TradeEconomicWeight)
        );
    }

    private static int CalculatePeaceScore(
        AiFactionContext faction,
        AiFactionContext opponentFaction,
        DiplomacyRelation relation
    )
    {
        var powerRatio = (float)faction.MilitaryPower / opponentFaction.MilitaryPower;
        var powerScore = float.Clamp((powerRatio - 1f) * 100f, 0f, 100f);

        var standingScore = (relation.Standing + 100) * 0.5f;

        return (int)(
            (powerScore * faction.AiPersonality.Diplomacy.PeaceDefeatWeight) +
            (standingScore * faction.AiPersonality.Diplomacy.PeaceStandingWeight)
        );
    }
}