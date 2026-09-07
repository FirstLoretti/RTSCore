using MediatR;

namespace RTSCore.Application.Campaign.Commands.Diplomacy;

public record AcceptOfferCommand(Guid OfferId) : IRequest;