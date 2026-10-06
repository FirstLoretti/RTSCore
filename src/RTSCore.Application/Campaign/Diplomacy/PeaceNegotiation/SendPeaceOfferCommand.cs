using MediatR;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public record SendPeaceOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;