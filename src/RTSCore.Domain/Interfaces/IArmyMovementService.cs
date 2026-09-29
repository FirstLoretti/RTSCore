using System.Numerics;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IArmyMovementService
{
    ArmyMovementResult MoveTo(Army army, Vector2 destination);
}