using MediatR;

namespace RTSCore.Application.Campaign.Commands.Diplomacy;

public record RejectOfferCommand(Guid OfferId) : IRequest;