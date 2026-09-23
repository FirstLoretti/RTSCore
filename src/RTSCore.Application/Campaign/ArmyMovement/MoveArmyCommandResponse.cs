namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommandResponse(string ArmyId, float X, float Y, int MovementPoints);