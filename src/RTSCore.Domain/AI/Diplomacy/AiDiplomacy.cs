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

    public static AiDiplomacyDecisions EvaluateIncomingOffers(AiDiplomacyContext context)
    {
        var decisions = new AiDiplomacyDecisions();
        RespondToOffers(context, decisions);
        return decisions;
    }

    public static AiDiplomacyDecisions GenerateOutgoingOffers(AiDiplomacyContext context)
    {
        var decisions = new AiDiplomacyDecisions();
        GenerateOutgoingOffers(context, decisions);
        return decisions;
    }

    private static void GenerateOutgoingOffers(AiDiplomacyContext context, AiDiplomacyDecisions decisions)
    {
        foreach (var anotherFaction in context.AnotherFactions)
        {
            var relation = context.GetRelation(anotherFaction.FactionType);

            if (AiOfferEvaluator.IsDeclareWarProfitable(relation, context.FactionContext, context.AiPersonality, anotherFaction))
            {
                decisions.WarToDeclare.Add(anotherFaction.FactionType);
                continue;
            }
            if (AiOfferEvaluator.IsPeaceOfferProfitable(relation, context.FactionContext, context.AiPersonality, anotherFaction))
            {
                decisions.PeaceOfferToSend.Add(anotherFaction.FactionType);
            }
            if (AiOfferEvaluator.IsTradeOfferProfitable(relation, context.FactionContext, context.AiPersonality, anotherFaction))
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
            var opponent = context.GetOpponent(offer.Initiator);

            if (AiOfferEvaluator.IsOfferProfitable(offer, relation, context.FactionContext, context.AiPersonality, opponent))
            {
                decisions.OffersToAccept.Add(offer);
            }
            else
            {
                decisions.OffersToReject.Add(offer);
            }
        }
    }
}