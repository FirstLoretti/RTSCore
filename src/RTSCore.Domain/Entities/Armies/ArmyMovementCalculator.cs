using System.Numerics;

namespace RTSCore.Domain.Services.Armies;

public static class ArmyMovementCalculator
{
    public static int CalculateCost(Vector2 position, Vector2 destination, int unitDistanceCost)
    {
        var distance = Vector2.Distance(position, destination);
        return (int)(distance * unitDistanceCost);
    }
}