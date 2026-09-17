using System.Numerics;

using MediatR;

namespace RTSCore.Application.Army.Commands;

public record MoveArmyCommand(string ArmyId, float X, float Y) : IRequest<MoveArmyCommandResponse>;