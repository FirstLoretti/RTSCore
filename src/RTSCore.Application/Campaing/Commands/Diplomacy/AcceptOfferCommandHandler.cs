using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class AcceptOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AcceptOfferCommand>
{
    public async Task Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await unitOfWork.DiplomacyOfferRepository.GetOfferAsync(request.OfferId, cancellationToken);
        Guard.Against.NotFound(offer, request.OfferId);

        var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(
            offer.Initiator, offer.Target, cancellationToken
        );
        Guard.Against.NotFoundRelation(relation, offer.Initiator, offer.Target);

        if (offer.Type == OfferType.TradeAgreement)
        {
            relation.OpenTrade();
        }
        else
        {
            throw new NotImplementedException($"Логика для типа соглашений {offer.Type} не реализована");
        }

        offer.Accept();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}