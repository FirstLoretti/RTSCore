using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Diplomacy;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public sealed class AcceptOfferCommandHandler(
    IUnitOfWork unitOfWork,
    DiplomacyConfiguration config
) : IRequestHandler<AcceptOfferCommand, Result>
{
    public async Task<Result> Handle(AcceptOfferCommand request, CancellationToken ct)
    {
        var offer = await unitOfWork.DiplomacyOfferRepository.GetAsync(request.OfferId, ct);
        Guard.Against.NotFound(request.OfferId, offer);

        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(offer.CampaignId, offer.Initiator, offer.Target, ct);
        Guard.Against.NotFound(offer.Initiator, relation);

        var result = offer.Type switch
        {
            OfferType.TradeAgreement => relation.OpenTrade(config.AcceptTradeOfferBonus),
            OfferType.PeaceTreaty => relation.MakePeace(config.AcceptPeaceOfferBonus),
            _ => throw new InvalidOperationException($"Offer:{offer.Type} do not added in accept offer handler logic")
        };

        if (result.IsFailure) return result;

        offer.Accept();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}