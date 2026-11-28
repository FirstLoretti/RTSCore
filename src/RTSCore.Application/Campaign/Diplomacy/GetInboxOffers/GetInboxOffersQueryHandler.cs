using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Campaign.Diplomacy.GetInboxOffers;

public sealed class GetInboxOffersQueryHandler(
    IDiplomacyOfferRepository offerRepository
) : IRequestHandler<GetInboxOffersQuery, IReadOnlyList<DiplomacyOfferDto>>
{
    public async Task<IReadOnlyList<DiplomacyOfferDto>> Handle(GetInboxOffersQuery request, CancellationToken ct)
    {
        var offers = await offerRepository.GetIncomingPendingOffersAsync(request.CampaignId, request.FactionType, ct, false);

        return [.. offers.Select(o => new DiplomacyOfferDto(o.Id, o.Initiator, o.Type))];
    }
}