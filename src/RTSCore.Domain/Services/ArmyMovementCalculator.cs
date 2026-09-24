using System.Numerics;

namespace RTSCore.Domain.Services;

public static class ArmyMovementCalculator
{
    public static int CalculateCost(Vector2 initial, Vector2 destination, int unitDistanceCost)
    {
        var distance = Vector2.Distance(initial, destination);
        return (int)(distance * unitDistanceCost);
    }
}