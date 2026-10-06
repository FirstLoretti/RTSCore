using MediatR;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public record SendTradeOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;