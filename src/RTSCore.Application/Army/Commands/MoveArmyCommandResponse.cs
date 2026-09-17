using System.Numerics;

namespace RTSCore.Application.Army.Commands;

public record MoveArmyCommandResponse(string ArmyId, Vector2 Coordinates, int MovementPoints);