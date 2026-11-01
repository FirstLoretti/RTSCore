using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public sealed class SendPeaceOfferCommanHandler(IUnitOfWork unitOfWork) : IRequestHandler<SendPeaceOfferCommand, DiplomacyOfferId>
{
    public async Task<DiplomacyOfferId> Handle(SendPeaceOfferCommand request, CancellationToken ct)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, request.Initiator, request.Target, ct);
        Guard.Against.NotFound(request.Initiator, relation);

        var offer = new DiplomacyOffer(request.CampaignId, request.Initiator, request.Target, OfferType.PeaceTreaty);

        unitOfWork.DiplomacyOfferRepository.Add(offer);
        await unitOfWork.SaveChangesAsync(ct);

        return offer.Id;
    }
}