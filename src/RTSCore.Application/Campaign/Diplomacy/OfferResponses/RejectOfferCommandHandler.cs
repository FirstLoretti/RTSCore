using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Services;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public class RejectOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RejectOfferCommand>
{
    public async Task Handle(RejectOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await unitOfWork.DiplomacyOfferRepository.GetAsync(request.OfferId, cancellationToken);
        Guard.Against.NotFound(offer, request.OfferId);

        offer.Reject();

        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(
            offer.Initiator, offer.Target, cancellationToken);
        Guard.Against.NotFoundRelation(relation, offer.Initiator, offer.Target);

        relation.ChangeStanding(GameBalance.Diplomacy.RejectOfferPenalty);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}