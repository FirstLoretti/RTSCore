using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public record DeclareWarCommand(FactionType Initiator, FactionType Target) : IRequest;