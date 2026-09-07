using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Commands.Diplomacy;

public record SendPeaceOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;