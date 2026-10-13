using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Enums;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public class AcceptOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AcceptOfferCommand>
{
    public async Task Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await unitOfWork.DiplomacyOfferRepository.GetAsync(request.OfferId, cancellationToken);
        Guard.Against.NotFound(offer, request.OfferId);

        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(
            offer.Initiator, offer.Target, cancellationToken
        );
        Guard.Against.NotFoundRelation(relation, offer.Initiator, offer.Target);

        if (offer.Type == OfferType.TradeAgreement)
        {
            relation.OpenTrade();
        }
        else if (offer.Type == OfferType.PeaceTreaty)
        {
            relation.MakePeace();
        }
        else
        {
            throw new NotImplementedException($"Логика для типа соглашений {offer.Type} не реализована");
        }

        offer.Accept();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}