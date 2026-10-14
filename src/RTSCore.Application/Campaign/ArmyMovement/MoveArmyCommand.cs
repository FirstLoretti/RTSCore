using MediatR;

using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommand(ArmyId Id, float X, float Y) : IRequest<ArmyMovementResult>;