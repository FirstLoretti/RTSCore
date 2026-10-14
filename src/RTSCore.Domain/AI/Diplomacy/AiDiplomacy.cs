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
        foreach (var opponent in context.OpponentsFactions)
        {
            var relation = context.GetRelation(opponent.FactionType);

            if (AiOfferEvaluator.IsDeclareWarProfitable(relation, context.Faction, opponent))
            {
                decisions.WarToDeclare.Add(opponent.FactionType);
            }
            if (AiOfferEvaluator.IsPeaceOfferProfitable(relation, context.Faction, opponent))
            {
                decisions.PeaceOfferToSend.Add(opponent.FactionType);
            }
            if (AiOfferEvaluator.IsTradeOfferProfitable(relation, context.Faction, opponent))
            {
                decisions.TradeOfferToSend.Add(opponent.FactionType);
            }
        }
    }

    private static void RespondToOffers(AiDiplomacyContext context, AiDiplomacyDecisions decisions)
    {
        foreach (var offer in context.IncomingOffers)
        {
            var relation = context.GetRelation(offer.Initiator);
            var opponent = context.OpponentsFactions.First(f => f.FactionType == offer.Initiator);

            if (AiOfferEvaluator.IsOfferProfitable(offer, relation, context.Faction, opponent))
                decisions.OffersToAccept.Add(offer);
            else
                decisions.OffersToReject.Add(offer);
        }
    }
}