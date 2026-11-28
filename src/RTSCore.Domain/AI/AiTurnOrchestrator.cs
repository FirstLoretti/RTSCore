using RTSCore.Domain.AI.Construction;
using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.AI.Recruitment;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Diplomacy;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.AI;

public sealed class AiTurnOrchestrator(
    AiCatalog aiCatalog,
    UnitCatalog unitCatalog,
    AiRecruiter aiRecruiter,
    AiConstructor aiConstructor,
    DiplomacyConfiguration diplomacyConfig
)
{
    public void SimulateTurn(AiFactionAgent ai, CampaignWorldState worldState)
    {
        DiplomacyProcess(ai, worldState);

        var cities = worldState.GetCities(ai.FactionType);
        var citiesByCoords = cities.ToDictionary(c => c.Coordinates);
        var faction = worldState.GetFaction(ai.FactionType);
        var armies = worldState.GetArmies(ai.FactionType);

        aiConstructor.StartConstruction(
            faction,
            ai.PersonalityType,
            cities
        );

        foreach (var army in armies)
        {
            if (!citiesByCoords.TryGetValue(army.Coordinates, out var city)) continue;

            var availableUnits = city.GetRecruitableUnits(unitCatalog.AllTemplates);

            aiRecruiter.StartRecruitment(faction, ai.PersonalityType, [army], availableUnits);
        }
    }

    private void DiplomacyProcess(AiFactionAgent ai, CampaignWorldState worldState)
    {
        var relations = worldState.GetRelations(ai.FactionType);

        var diplomacyContext = new AiDiplomacyContext(
            worldState.GetFactionContext(ai.FactionType),
            aiCatalog.GetPersonality(ai.PersonalityType),
            [.. worldState.AllFactionContexts.Where(c => c.FactionType != ai.FactionType)],
            relations,
            worldState.GetPendingOffers(ai.FactionType)
        );

        var incomingDecisions = AiDiplomacy.EvaluateIncomingOffers(diplomacyContext);

        foreach (var offer in incomingDecisions.OffersToAccept)
        {
            if (offer.Accept().IsSuccess)
            {
                var relation = worldState.GetRelation(offer.Initiator, ai.FactionType);

                if (offer.Type == OfferType.TradeAgreement) relation.OpenTrade(diplomacyConfig.AcceptTradeOfferBonus);
                if (offer.Type == OfferType.PeaceTreaty) relation.MakePeace(diplomacyConfig.AcceptPeaceOfferBonus);
            }
        }
        foreach (var offer in incomingDecisions.OffersToReject) offer.Reject();

        var outgoingOffers = AiDiplomacy.GenerateOutgoingOffers(diplomacyContext);

        foreach (var faction in outgoingOffers.PeaceOfferToSend)
        {
            var relation = worldState.GetRelation(faction, ai.FactionType);

            var result = relation.CreatePeaceOffer(ai.FactionType, faction);
            if (result.IsSuccess) worldState.AddNewOffer(result.Value);
        }
        foreach (var faction in outgoingOffers.TradeOfferToSend)
        {
            var relation = worldState.GetRelation(faction, ai.FactionType);

            var result = relation.CreateTradeOffer(ai.FactionType, faction);
            if (result.IsSuccess) worldState.AddNewOffer(result.Value);
        }
        foreach (var faction in outgoingOffers.WarToDeclare)
        {
            var relation = worldState.GetRelation(faction, ai.FactionType);
            relation.DeclareWar(diplomacyConfig.DeclareWarPenalty);
        }
    }
}