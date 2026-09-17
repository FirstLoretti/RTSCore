namespace RTSCore.Application.Army.Commands;

public record MoveArmyCommandResponse(string ArmyId, float X, float Y, int MovementPoints);