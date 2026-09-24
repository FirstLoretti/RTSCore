using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommand(ArmyId ArmyId, float X, float Y) : IRequest<MoveArmyCommandResponse>;