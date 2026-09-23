using System.Numerics;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Army
{
    public string Id { get; init; } = string.Empty;
    public FactionType Faction { get; init; }
    public UnitId GeneralId { get; private set; } // TODO: Убрать
    public Vector2 Coordinates { get; private set; }
    public int MovementPoints { get; private set; }

    private readonly List<Unit> _units = [];
    public IReadOnlyList<Unit> Units => _units.AsReadOnly();

    private const int MaxSize = 20;
    public bool HasFreeSlots => _units.Count < MaxSize;

    private Army(
        FactionType faction,
        Vector2 coordinates,
        UnitTemplate general
    )
    {
        Id = $"army_{Guid.NewGuid()}";
        Faction = faction;
        Coordinates = coordinates;

        _units.Add(new Unit(faction, general, Id));
    }

    private Army() { }

    public static Army Create(FactionType faction, Vector2 coordinates, UnitTemplate general)
        => new(faction, coordinates, general);

    public void MoveTo(Vector2 destination, int movementCost)
    {
        var absCost = int.Abs(movementCost);
        if (MovementPoints < absCost) throw new GameRuleException("Недостаточно очков перемещения.");

        MovementPoints -= absCost;
        Coordinates = destination;
    }

    public void RestoreMovementPoints(int maxPoints) => MovementPoints = int.Max(0, maxPoints);

    public void RecruitUnit(UnitTemplate unit)
    {
        if (!HasFreeSlots) throw new GameRuleException("Нельзя нанять юнита, армия уже укомплектована.");

        _units.Add(new Unit(Faction, unit, Id));
    }

    public void DisbandUnit(Unit unit) => _units.Remove(unit);

    public void PurgeDeadUnits()
    {
        var deadUnits = _units.Where(u => !u.IsAlive).ToList();
        foreach (var unit in deadUnits)
        {
            _units.Remove(unit);
        }
    }
}