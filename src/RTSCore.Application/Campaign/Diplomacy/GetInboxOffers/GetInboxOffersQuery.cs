using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Campaign.Diplomacy.GetInboxOffers;

public sealed record GetInboxOffersQuery(
    CampaignId CampaignId,
    FactionType FactionType
) : IRequest<IReadOnlyList<DiplomacyOfferDto>>;