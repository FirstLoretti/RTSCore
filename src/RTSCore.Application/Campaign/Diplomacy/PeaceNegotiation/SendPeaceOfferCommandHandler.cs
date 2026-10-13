using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Enums;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public class SendPeaceOfferCommanHandler(IUnitOfWork unitOfWork) : IRequestHandler<SendPeaceOfferCommand, Guid>
{
    public async Task<Guid> Handle(SendPeaceOfferCommand request, CancellationToken cancellationToken)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(
            request.Initiator, request.Target, cancellationToken
        );
        Guard.Against.NotFoundRelation(relation, request.Initiator, request.Target);

        relation.ThrowIfCannotProposePeaceOffer();

        var factionUnderNegotiation = await unitOfWork.DiplomacyOfferRepository.GetFactionsUnderNegotiationAsync(
            request.Initiator, cancellationToken
        );
        Guard.Against.DuplicateOffer(factionUnderNegotiation, request.Target);

        var offer = new DiplomacyOffer(request.Initiator, request.Target, OfferType.PeaceTreaty);

        unitOfWork.DiplomacyOfferRepository.Add(offer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return offer.Id;
    }
}