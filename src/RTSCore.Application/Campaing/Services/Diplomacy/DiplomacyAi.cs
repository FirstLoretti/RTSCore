using MediatR;

using RTSCore.Application.Campaing.Commands.Diplomacy;
using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

using Ai = RTSCore.Domain.Services.GameBalance.Diplomacy.Ai;

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

        await RespondToIncomingOffers(aiFaction, factionToCitiesCount, factionToMilitaryPower, cancellationToken);
        await GenerateOutgoingOffers(
            aiFaction, [.. otherFactions], factionToCitiesCount, factionToMilitaryPower, cancellationToken
        );
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

            var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(
                aiFaction, targetFaction, cancellationToken
            );
            Guard.Against.NotFoundRelation(relation, aiFaction, targetFaction);

            if (relation.InWar)
            {
                var isPeaceNeeded = await EvaluatePeaceOfferUtilityAsync(
                    aiFaction, targetFaction, factionToMilitaryPower, cancellationToken
                );

                if (isPeaceNeeded)
                {
                    await mediator.Send(new SendPeaceOfferCommand(aiFaction, targetFaction), cancellationToken);
                }

                continue;
            }

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
        Dictionary<FactionType, int> factionToMilitaryPower,
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
            else if (offer.Type == OfferType.PeaceTreaty)
            {
                bool isPeaceNeeded = await EvaluatePeaceOfferUtilityAsync(
                    aiFaction, offer.Initiator, factionToMilitaryPower, cancellationToken
                );

                if (isPeaceNeeded)
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
            factionCityCount * Ai.TradeScorePerTargetCity, DiplomacyRelation.MaxStanding
        );

        var finalScore = (standingScore * Ai.TradeStandingWeight) + (economicScore * Ai.TradeEconomicWeight);

        return finalScore >= Ai.TradeOfferThreshold;
    }

    private async Task<bool> EvaluatePeaceOfferUtilityAsync(
        FactionType aiFaction,
        FactionType targetFaction,
        Dictionary<FactionType, int> factionToMilitaryPower,
        CancellationToken cancellationToken
    )
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(
            aiFaction, targetFaction, cancellationToken
        );
        Guard.Against.NotFoundRelation(relation, aiFaction, targetFaction);

        if (!relation.InWar) return false;

        var targetPower = factionToMilitaryPower.GetValueOrDefault(targetFaction);
        if (targetPower <= 0) return false;

        var myPower = factionToMilitaryPower.GetValueOrDefault(aiFaction);
        var powerRatio = (float)myPower / targetPower;

        var defeatScore = powerRatio < Ai.PeaceDesperationRatioThreshold
            ? float.Clamp((1.0f - powerRatio) * 100, 0, 100)
            : 0;

        var standingScore = (relation.Standing + 100) * 0.5f;

        var finalScore = (defeatScore * Ai.PeaceDefeatWeight) + (standingScore * Ai.PeaceStandingWeight);

        return finalScore > Ai.PeaceOfferThreshold;
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

        var finalScore = (hostilityScore * Ai.WarHostilityWeight) + (weaknessScore * Ai.WarWeaknessWeight);

        return finalScore > Ai.WarDeclarationThreshold;
    }
}