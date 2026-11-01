using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public sealed class SendTradeOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<SendTradeOfferCommand, DiplomacyOfferId>
{
    public async Task<DiplomacyOfferId> Handle(SendTradeOfferCommand request, CancellationToken ct)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, request.Initiator, request.Target, ct);
        Guard.Against.NotFound(request.Initiator, relation);

        var offer = new DiplomacyOffer(relation.CampaignId, request.Initiator, request.Target, OfferType.TradeAgreement);

        unitOfWork.DiplomacyOfferRepository.Add(offer);
        await unitOfWork.SaveChangesAsync(ct);

        return offer.Id;
    }
}