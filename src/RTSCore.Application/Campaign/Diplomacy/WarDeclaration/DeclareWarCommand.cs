using MediatR;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public record DeclareWarCommand(FactionType Initiator, FactionType Target) : IRequest;