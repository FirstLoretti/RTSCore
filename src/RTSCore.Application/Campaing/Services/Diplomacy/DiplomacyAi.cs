using MediatR;

using RTSCore.Application.Campaing.Commands.Diplomacy;
using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Services.Diplomacy;

public class DiplomacyAi(IUnitOfWork unitOfWork, IMediator mediator)
{
    public async Task ProcessTurnAsync(FactionType aiFaction, CancellationToken cancellationToken)
    {
        var otherFactions = await unitOfWork.FactionRepository.GetAnotherFactionsAsync(aiFaction, cancellationToken);
        var factionToCitiesCount = await unitOfWork.CityRepository.GetFactionToCitiesCount(otherFactions, cancellationToken);

        await RespondToIncomingOffers(aiFaction, factionToCitiesCount, cancellationToken);
        await GenerateOutgoingOffers(aiFaction, [.. otherFactions], factionToCitiesCount, cancellationToken);
    }

    private async Task<bool> EvaluateTradeOfferUtilityAsync(
        FactionType aiFaction,
        FactionType targetFaction,
        Dictionary<FactionType, int> factionToCitiesCount,
        CancellationToken cancellationToken
    )
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(aiFaction, targetFaction, cancellationToken);
        Guard.Against.NotFoundRelation(relation, aiFaction, targetFaction);

        if (relation.HasTradeAgreement) return false;
        if (relation.Standing < GameBalance.Diplomacy.MinStandingForTrade) return false;

        var standingScore = (int)((relation.Standing + DiplomacyRelation.MaxStanding) * 0.5f);

        var factionCityCount = factionToCitiesCount.GetValueOrDefault(targetFaction);
        var economicScore = Math.Min(
            factionCityCount * GameBalance.Diplomacy.Ai.ScorePerTargetCity, DiplomacyRelation.MaxStanding
        );

        var totalScore =
            (standingScore * GameBalance.Diplomacy.Ai.StandingWeight) +
            (economicScore * GameBalance.Diplomacy.Ai.EconomicWeight);

        return totalScore >= GameBalance.Diplomacy.Ai.TradeOfferThreshold;
    }

    private async Task GenerateOutgoingOffers(
        FactionType aiFaction,
        FactionType[] otherFactions,
        Dictionary<FactionType, int> factionToCitiesCount,
        CancellationToken cancellationToken)
    {
        var factionsUnderNegotiations =
            await unitOfWork.DiplomacyOfferRepository.GetFactionsUnderNegotiationAsync(aiFaction, cancellationToken);

        foreach (var targetFaction in otherFactions)
        {
            if (factionsUnderNegotiations.Contains(targetFaction)) continue;

            var isProfitable = await EvaluateTradeOfferUtilityAsync(
                aiFaction, targetFaction, factionToCitiesCount, cancellationToken
            );

            if (isProfitable)
            {
                await mediator.Send(new SendTradeOfferCommand(aiFaction, targetFaction), cancellationToken);
            }
        }
    }

    private async Task RespondToIncomingOffers(
        FactionType aiFaction,
        Dictionary<FactionType, int> factionToCitiesCount,
        CancellationToken cancellationToken)
    {
        var incomingOffers = await unitOfWork.DiplomacyOfferRepository.GetFactionOffersAsync(aiFaction, cancellationToken);

        var myInbox = incomingOffers.Where(o => o.Target == aiFaction);

        foreach (var offer in myInbox)
        {
            if (offer.Type == OfferType.TradeAgreement)
            {
                bool isProfitable = await EvaluateTradeOfferUtilityAsync(
                    aiFaction, offer.Initiator, factionToCitiesCount, cancellationToken
                );

                if (isProfitable)
                {
                    await mediator.Send(new AcceptOfferCommand(offer.Id), cancellationToken);
                }
                else
                {
                    await mediator.Send(new RejectOfferCommand(offer.Id), cancellationToken);
                }
            }
        }
    }
}