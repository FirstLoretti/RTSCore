using System.Numerics;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Army
{
    public string Id { get; init; }
    public FactionType Faction { get; init; }

    public UnitId GeneralId { get; private set; }
    public Vector2 Coordinates { get; private set; }
    public int MovementPoints { get; private set; }
    public int MaxMovementPoints { get; private set; }
    public int MaxSize { get; private set; }
    public bool HasGeneral { get; private set; }

    private readonly List<Unit> _units = [];
    public IReadOnlyList<Unit> Units => _units.AsReadOnly();

    public bool HasFreeSlots => MaxSize > _units.Count;
    public int Size => _units.Count;

    private Army(
        string id,
        FactionType faction,
        Vector2 coordinates,
        int maxMovementPoints,
        int maxSize,
        bool hasGeneral)
    {
        Id = id;
        Faction = faction;
        Coordinates = coordinates;
        MovementPoints = maxMovementPoints;
        MaxMovementPoints = maxMovementPoints;
        MaxSize = maxSize;
        HasGeneral = hasGeneral;
    }

    private Army() : this(null!, default, default, default, default, default) { }

    public static Army Create(
        FactionType faction,
        Vector2 coordinates,
        int maxMovementPoints,
        int maxSize,
        Unit general
    )
    {
        var id = $"army_{Guid.NewGuid()}";
        var army = new Army(id, faction, coordinates, maxMovementPoints, maxSize, hasGeneral: true);

        army.AssignGeneral(general);

        return army;
    }

    public static Army CreateWithoutGeneral(
        FactionType faction,
        Vector2 coordinates,
        int maxMovementPoints,
        int maxSize
    )
    {
        var id = $"army_{Guid.NewGuid()}";
        return new Army(id, faction, coordinates, maxMovementPoints, maxSize, hasGeneral: false);
    }

    public void MoveTo(Vector2 destination, int movementCost)
    {
        if (MovementPoints < movementCost) throw new GameRuleException("Недостаточно очков перемещения.");

        MovementPoints -= movementCost;
        Coordinates = destination;
    }

    public void RecruitUnit(Unit unit)
    {
        if (!HasFreeSlots) throw new GameRuleException("Нельзя нанять юнита, армия уже укомплектована.");
        if (unit.Faction != Faction) throw new GameRuleException("Нельзя нанять юнита, нанятого чужой фракцией.");

        unit.AssignToArmy(Id);
        _units.Add(unit);
    }

    public void AssignUnit(Unit unit)
    {
        if (unit.Faction != Faction) throw new GameRuleException("Нельзя нанять юнита, нанятого чужой фракцией.");

        _units.Add(unit);
        unit.AssignToArmy(Id);
    }

    public void PurgeDeadUnits()
    {
        var deadUnits = _units.Where(u => !u.IsAlive).ToList();
        foreach (var unit in deadUnits)
        {
            _units.Remove(unit);
        }
    }

    private void AssignGeneral(Unit general)
    {
        if (general.Faction != Faction) throw new GameRuleException("Нельзя нанять генерала, нанятого чужой фракцией.");

        GeneralId = general.Id;
        _units.Add(general);
        general.AssignToArmy(Id);
        HasGeneral = true;
    }
}