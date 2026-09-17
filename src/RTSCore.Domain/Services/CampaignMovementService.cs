using System.Numerics;

using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.Services;

public class CampaignMovementService : ICampaignMovementService
{
    private const float MovementCostPerUnitDistance = 10f;

    public int CalculateMovementCost(Vector2 initial, Vector2 destination)
    {
        var distance = Vector2.Distance(initial, destination);
        return (int)(distance * MovementCostPerUnitDistance);
    }
}