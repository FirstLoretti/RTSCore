using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class RejectOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RejectOfferCommand>
{
    public async Task Handle(RejectOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await unitOfWork.DiplomacyOfferRepository.GetOfferAsync(request.OfferId, cancellationToken);
        Guard.Against.NotFound(offer, request.OfferId);

        offer.Reject();

        var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(
            offer.Initiator, offer.Target, cancellationToken);
        Guard.Against.NotFoundRelation(relation, offer.Initiator, offer.Target);

        relation.ChangeStanding(GameBalance.Diplomacy.RejectOfferPenalty);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}