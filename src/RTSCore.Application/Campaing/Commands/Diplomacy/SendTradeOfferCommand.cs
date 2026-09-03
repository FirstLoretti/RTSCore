using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public record SendTradeOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;