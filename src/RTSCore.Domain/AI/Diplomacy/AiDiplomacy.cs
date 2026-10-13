using RTSCore.Application.Campaign.Diplomacy.OfferResponses;
using RTSCore.Application.Campaign.Diplomacy.TradeProposal;
using RTSCore.Application.Campaign.Diplomacy.WarDeclaration;
using RTSCore.Domain.AI;
using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Application.AI.Diplomacy;

public sealed class AiDiplomacy(

    IUnitOfWork unitOfWork
)
{
    public async Task HandleDiplomacyLogicAsync(FactionType factionType, CancellationToken ct)
    {
        var factions = await unitOfWork.FactionRepository.GetAsync(ct);
        var faction = factions.FirstOrDefault(f => f.Type == factionType);
        var otherFactions = factions
            .Where(f => f.Type != factionType)
            .Select(f => f.Type)
            .ToList();

        var factionToCityCount = await unitOfWork.CityRepository
            .GetFactionToCityCountAsync(otherFactions, ct);
        var factionToMilitaryPower = await unitOfWork.FactionRepository
            .GetFactionToMilitaryPowerAsync(allFactions, ct);

        var relations = await unitOfWork.DiplomacyRelationRepository.GetAsync(faction, ct);

        var context = new AiFactionContext(faction,)

        await RespondToOffers(faction, factionToCityCount, factionToMilitaryPower, ct);
        await GenerateOutgoingOffers(faction, otherFactions, relations, factionToCityCount, factionToMilitaryPower, ct);
    }

    private async Task GenerateOutgoingOffers(
        FactionType faction,
        IReadOnlyCollection<FactionType> otherFactions,
        IReadOnlyCollection<DiplomacyRelation> relations,
        AiFactionContext context
    )
    {
        var factionsUnderNegotiations = await unitOfWork.DiplomacyOfferRepository
            .GetFactionsUnderNegotiationAsync(faction, ct);

        foreach (var faction in otherFactions)
        {
            if (factionsUnderNegotiations.Contains(faction)) continue;

            var factionContext = new AiFactionContext(ai)
            var peaseOffer = AiOffersEvaluator.IsPeaceOfferProfitable(relations,)

            var isWarProfitable = await EvaluateDeclareWarUtilityAsync(
                faction, faction, factionToMilitaryPower, ct
            );
            if (isWarProfitable)
            {
                await mediator.Send(new DeclareWarCommand(faction, faction), ct);
                continue;
            }

            var isTradeProfitable = await EvaluateTradeOfferUtilityAsync(
                faction, faction, factionToCityCount, ct
            );
            if (isTradeProfitable)
            {
                await mediator.Send(new SendTradeOfferCommand(faction, faction), ct);
            }
        }
    }

    private void RespondToOffers(
        Faction faction,
        Faction[] otherFactions,
        DiplomacyRelation[] relations,
        DiplomacyOffer[] offers
    )
    {
        var context = new AiFactionContext(faction, faction.AiPersonalityType, faction.CitiesCount, faction.)
        foreach (var offer in offers)
        {
            var relation = relations.Where(r => r.FactionA == offer.Initiator || r.FactionB == offer.Initiator);
            var initiatorContext = new AiFactionContext()
            AiOffersEvaluator.IsOfferProfitable(relation, offer, context,
        }
    }
}