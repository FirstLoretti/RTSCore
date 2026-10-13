using MediatR;

using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommand(ArmyId Id, float X, float Y) : IRequest<ArmyMovementResult>;