using MediatR;

namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommand(string ArmyId, float X, float Y) : IRequest<MoveArmyCommandResponse>;