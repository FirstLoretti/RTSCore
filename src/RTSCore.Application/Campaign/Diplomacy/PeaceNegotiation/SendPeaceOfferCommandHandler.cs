using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public sealed class SendPeaceOfferCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<SendPeaceOfferCommand, Result<DiplomacyOfferId>>
{
    public async Task<Result<DiplomacyOfferId>> Handle(SendPeaceOfferCommand request, CancellationToken ct)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, request.Initiator, request.Target, ct);
        Guard.Against.NotFound(request.Initiator, relation);

        var offer = relation.CreatePeaceOffer(request.Initiator, request.Target);
        if (offer.IsFailure) return offer.Error;

        unitOfWork.DiplomacyOfferRepository.Add(offer.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return offer.Value.Id;
    }
}