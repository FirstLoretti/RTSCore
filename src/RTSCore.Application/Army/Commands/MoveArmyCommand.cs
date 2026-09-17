using System.Numerics;

using MediatR;

namespace RTSCore.Application.Army.Commands;

public record MoveArmyCommand(string ArmyId, Vector2 Destination) : IRequest<MoveArmyCommandResponse>;