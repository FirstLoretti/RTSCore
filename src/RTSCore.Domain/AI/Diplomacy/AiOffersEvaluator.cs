using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Domain.AI.Diplomacy;

public static class AiOffersEvaluator
{
    public static bool IsOfferProfitable(
        DiplomacyRelation relation,
        DiplomacyOffer offer,
        AiFactionContext context,
        AiFactionContext partnerContext
    )
    {
        if (offer.Type == OfferType.TradeAgreement)
            return IsTradeOfferProfitable(relation, context, partnerContext);

        else if (offer.Type == OfferType.PeaceTreaty)
            return IsPeaceOfferProfitable(relation, context, partnerContext);

        return false;
    }

    private static bool IsTradeOfferProfitable(
        DiplomacyRelation relation,
        AiFactionContext context,
        AiFactionContext partnerContext
    )
    {
        if (relation.HasTradeAgreement) return false;

        if (relation.Standing < context.Personality.Diplomacy.TradeThreshold) return false;

        var score = CalculateTradeScore(relation, partnerContext, context.Personality);

        return score > context.Personality.Diplomacy.TradeThreshold;
    }

    private static bool IsPeaceOfferProfitable(
       DiplomacyRelation relation,
       AiFactionContext context,
       AiFactionContext partnerContext
    )
    {
        if (!relation.InWar || partnerContext.MilitaryPower <= 0) return false;
        if (context.MilitaryPower <= 0) return true;

        var score = CalculatePeaceScore(context, partnerContext, relation, context.Personality);

        return score > context.Personality.Diplomacy.PeaсeThreshold;
    }

    private static bool IsDeclareWarProfitable(
        DiplomacyRelation relation,
        AiFactionContext context,
        AiFactionContext partnerContext
    )
    {
        if (relation.InWar || context.MilitaryPower <= 0) return false;
        if (partnerContext.MilitaryPower <= 0) return true;

        var score = CalculateWarScore(relation, context, partnerContext);

        return score > context.Personality.Diplomacy.WarThreshold;
    }

    private static int CalculateWarScore(
        DiplomacyRelation relation,
        AiFactionContext context,
        AiFactionContext partnerContext
    )
    {
        var relationScore = (DiplomacyRelation.MaxStanding - relation.Standing) * 0.5f;

        var powerRatio = (float)context.MilitaryPower / partnerContext.MilitaryPower;
        var powerScore = float.Clamp((powerRatio - 1f) * 100f, 0f, 100f);

        return (int)(
            (relationScore * context.Personality.Diplomacy.WarRelationWeight) +
            (powerScore * context.Personality.Diplomacy.WarPowerWeight)
        );
    }

    private static int CalculateTradeScore(
        DiplomacyRelation relation,
        AiFactionContext partnerContext,
        AiPersonality personality
    )
    {
        var standingScore = (int)((relation.Standing + DiplomacyRelation.MaxStanding) * 0.5f);

        var economicScore = Math.Min(
            partnerContext.CitiesCount * personality.Diplomacy.TradeScorePerPartnerCity,
            DiplomacyRelation.MaxStanding
        );

        return (int)(
            (standingScore * personality.Diplomacy.TradeStandingWeight) +
            (economicScore * personality.Diplomacy.TradeEconomicWeight)
        );
    }

    private static int CalculatePeaceScore(
        AiFactionContext context,
        AiFactionContext partnerContext,
        DiplomacyRelation relation,
        AiPersonality personality
    )
    {
        var powerRatio = (float)context.MilitaryPower / partnerContext.MilitaryPower;

        var powerScore = float.Clamp((powerRatio - 1f) * 100f, 0f, 100f);

        var standingScore = (relation.Standing + 100) * 0.5f;

        return (int)(
            (powerScore * personality.Diplomacy.PeaceDefeatWeight) +
            (standingScore * personality.Diplomacy.PeaceStandingWeight)
        );
    }
}