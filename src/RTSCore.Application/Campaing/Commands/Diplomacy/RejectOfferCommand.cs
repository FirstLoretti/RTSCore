using MediatR;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public record RejectOfferCommand(Guid OfferId) : IRequest;