using MediatR;

using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.ValueObjects.Results;

namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommand(ArmyId Id, float X, float Y) : IRequest<ArmyMovementResult>;