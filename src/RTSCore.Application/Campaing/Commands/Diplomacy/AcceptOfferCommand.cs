using MediatR;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public record AcceptOfferCommand(Guid OfferId) : IRequest;