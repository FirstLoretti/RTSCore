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
        var allFactions = otherFactions.Concat([aiFaction]);
        var factionToCitiesCount = await unitOfWork.CityRepository.GetFactionToCitiesCount(otherFactions, cancellationToken);
        var factionToMilitaryPower =
            await unitOfWork.FactionRepository.GetFactionToMilitaryPower(allFactions, cancellationToken
        );

        await RespondToIncomingOffers(aiFaction, factionToCitiesCount, cancellationToken);
        await GenerateOutgoingOffers(
            aiFaction, [.. otherFactions], factionToCitiesCount, factionToMilitaryPower, cancellationToken
        );
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

        var finalScore =
            (standingScore * GameBalance.Diplomacy.Ai.StandingWeight) +
            (economicScore * GameBalance.Diplomacy.Ai.EconomicWeight);

        return finalScore >= GameBalance.Diplomacy.Ai.TradeOfferThreshold;
    }

    private async Task<bool> EvaluateDeclareWarUtilityAsync(
        FactionType aiFaction,
        FactionType targetFaction,
        Dictionary<FactionType, int> factionToMilitaryPower,
        CancellationToken cancellationToken
    )
    {
        var myPower = factionToMilitaryPower.GetValueOrDefault(aiFaction);
        if (myPower <= 0) return false;

        var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(aiFaction, targetFaction, cancellationToken);
        Guard.Against.NotFoundRelation(relation, aiFaction, targetFaction);

        if (relation.InWar) return false;

        var hostilityScore = (DiplomacyRelation.MaxStanding - relation.Standing) * 0.5f;
        var targetPower = factionToMilitaryPower.GetValueOrDefault(targetFaction);
        var powerRatio = (float)targetPower / myPower;
        var weaknessScore = float.Clamp((1.0f - powerRatio) * 100, 0f, 100f);

        var finalScore = (hostilityScore * GameBalance.Diplomacy.Ai.HostilityWeight) +
                         (weaknessScore * GameBalance.Diplomacy.Ai.WeaknessWeight);

        return finalScore > GameBalance.Diplomacy.Ai.WarDeclarationThreshold;
    }

    private async Task GenerateOutgoingOffers(
        FactionType aiFaction,
        FactionType[] otherFactions,
        Dictionary<FactionType, int> factionToCitiesCount,
        Dictionary<FactionType, int> factionToMilitaryPower,
        CancellationToken cancellationToken)
    {
        var factionsUnderNegotiations =
            await unitOfWork.DiplomacyOfferRepository.GetFactionsUnderNegotiationAsync(aiFaction, cancellationToken);

        foreach (var targetFaction in otherFactions)
        {
            if (factionsUnderNegotiations.Contains(targetFaction)) continue;

            var isWarProfitable = await EvaluateDeclareWarUtilityAsync(
                aiFaction, targetFaction, factionToMilitaryPower, cancellationToken
            );
            if (isWarProfitable)
            {
                await mediator.Send(new DeclareWarCommand(aiFaction, targetFaction), cancellationToken);
                continue;
            }

            var isTradeProfitable = await EvaluateTradeOfferUtilityAsync(
                aiFaction, targetFaction, factionToCitiesCount, cancellationToken
            );
            if (isTradeProfitable)
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