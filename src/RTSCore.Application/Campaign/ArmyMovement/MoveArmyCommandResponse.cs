using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.ArmyMovement;

public record MoveArmyCommandResponse(ArmyId ArmyId, float X, float Y, int MovementPoints);