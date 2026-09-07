using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Commands.Diplomacy;

public record DeclareWarCommand(FactionType Initiator, FactionType Target) : IRequest;