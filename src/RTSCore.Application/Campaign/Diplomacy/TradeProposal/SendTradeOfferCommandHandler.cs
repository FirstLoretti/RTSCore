using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Enums;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public class SendTradeOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<SendTradeOfferCommand, Guid>
{
    public async Task<Guid> Handle(SendTradeOfferCommand request, CancellationToken cancellationToken)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(
            request.Initiator, request.Target, cancellationToken);
        Guard.Against.NotFoundRelation(relation, request.Initiator, request.Target);

        relation.ThrowIfCannotProposeTradeOffer();

        var underNegotiation = await unitOfWork.DiplomacyOfferRepository.GetFactionsUnderNegotiationAsync(
            request.Initiator, cancellationToken
        );
        Guard.Against.DuplicateOffer(underNegotiation, request.Target);

        var offer = new DiplomacyOffer(request.Initiator, request.Target, OfferType.TradeAgreement);

        unitOfWork.DiplomacyOfferRepository.Add(offer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return offer.Id;
    }
}