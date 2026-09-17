using System.Numerics;

namespace RTSCore.Domain.Interfaces;

public interface ICampaignMovementService
{
    int CalculateMovementCost(Vector2 initial, Vector2 destination);
}