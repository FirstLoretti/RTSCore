using MediatR;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public record RejectOfferCommand(Guid OfferId) : IRequest;