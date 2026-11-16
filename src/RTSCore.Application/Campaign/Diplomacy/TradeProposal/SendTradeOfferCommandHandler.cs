using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public sealed class SendTradeOfferCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<SendTradeOfferCommand, Result<DiplomacyOfferId>>
{
    public async Task<Result<DiplomacyOfferId>> Handle(SendTradeOfferCommand request, CancellationToken ct)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, request.Initiator, request.Target, ct);
        Guard.Against.NotFound(request.Initiator, relation);

        var result = relation.CreateTradeOffer(request.Initiator, request.Target);
        if (result.IsFailure) return result.Error;

        unitOfWork.DiplomacyOfferRepository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return result.Value.Id;
    }
}