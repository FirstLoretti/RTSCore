using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public record DeclareWarCommand(FactionType Initiator, FactionType Target) : IRequest;