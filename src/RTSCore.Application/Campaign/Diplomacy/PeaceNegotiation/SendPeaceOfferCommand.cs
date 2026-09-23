using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public record SendPeaceOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;