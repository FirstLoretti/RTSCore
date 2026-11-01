using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public sealed record SendTradeOfferCommand(
    CampaignId CampaignId,
    FactionType Initiator,
    FactionType Target
) : IRequest<DiplomacyOfferId>;