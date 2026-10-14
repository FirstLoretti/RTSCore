using MediatR;

using RTSCore.Domain;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public record SendPeaceOfferCommand(FactionType Initiator, FactionType Target) : IRequest<Guid>;