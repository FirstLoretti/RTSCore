using MediatR;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public record AcceptOfferCommand(Guid OfferId) : IRequest;