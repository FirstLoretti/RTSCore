using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public sealed class RejectOfferCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RejectOfferCommand>
{
    public async Task Handle(RejectOfferCommand request, CancellationToken ct)
    {
        var offer = await unitOfWork.DiplomacyOfferRepository.GetAsync(request.OfferId, ct);
        Guard.Against.NotFound(request.OfferId, offer);

        offer.Reject();

        await unitOfWork.SaveChangesAsync(ct);
    }
}