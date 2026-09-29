using System.Numerics;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Domain.Services;

public class ArmyMovementService(ArmyConfiguration configuration) : IArmyMovementService
{
    public ArmyMovementResult MoveTo(Army army, Vector2 destination)
    {
        var movementCost = ArmyMovementCalculator.CalculateCost(
            army.Coordinates,
            destination,
            configuration.MovementUnitDistanceCost
        );

        army.MoveTo(destination, movementCost);

        return new ArmyMovementResult(army.Id, army.Coordinates.X, army.Coordinates.Y, army.MovementPoints);
    }
}