namespace RTSCore.Domain.AI.Diplomacy;

public static class AiDiplomacy
{
    public static AiDiplomacyDecisions ProcessDiplomacy(AiDiplomacyContext diplomacyContext)
    {
        var decisions = new AiDiplomacyDecisions();

        RespondToOffers(diplomacyContext, decisions);
        GenerateOutgoingOffers(diplomacyContext, decisions);

        return decisions;
    }

    private static void GenerateOutgoingOffers(AiDiplomacyContext context, AiDiplomacyDecisions decisions)
    {
        foreach (var anotherFaction in context.AnotherFactions)
        {
            var relation = context.GetRelation(anotherFaction.FactionType);

            if (AiOfferEvaluator.IsDeclareWarProfitable(relation, context.Faction, anotherFaction))
            {
                decisions.WarToDeclare.Add(anotherFaction.FactionType);
                continue;
            }
            if (AiOfferEvaluator.IsPeaceOfferProfitable(relation, context.Faction, anotherFaction))
            {
                decisions.PeaceOfferToSend.Add(anotherFaction.FactionType);
            }
            if (AiOfferEvaluator.IsTradeOfferProfitable(relation, context.Faction, anotherFaction))
            {
                decisions.TradeOfferToSend.Add(anotherFaction.FactionType);
            }
        }
    }

    private static void RespondToOffers(AiDiplomacyContext context, AiDiplomacyDecisions decisions)
    {
        foreach (var offer in context.IncomingOffers)
        {
            var relation = context.GetRelation(offer.Initiator);
            var opponent = GetOpponent(context, offer.Initiator);

            if (AiOfferEvaluator.IsOfferProfitable(offer, relation, context.Faction, opponent))
            {
                decisions.OffersToAccept.Add(offer);
            }
            else
            {
                decisions.OffersToReject.Add(offer);
            }
        }

        context.IncomingOffers.Clear();
    }

    private static AiFactionContext GetOpponent(AiDiplomacyContext context, FactionType initiator)
    {
        foreach (var faction in context.AnotherFactions)
        {
            if (faction.FactionType == initiator)
            {
                return faction;
            }
        }

        throw new KeyNotFoundException($"Faction {initiator} is missing from the world");
    }
}