using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public record SendTradeOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;