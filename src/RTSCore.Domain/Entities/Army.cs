using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Army
{
    public string Id { get; init; }
    public FactionType Faction { get; init; }

    public UnitId GeneralId { get; private set; }
    public Coordinates Coordinates { get; private set; }
    public int MovementPoints { get; private set; }
    public int MaxMovementPoints { get; private set; }
    public int MaxSize { get; private set; }

    private readonly List<Unit> _units = [];
    public IReadOnlyList<Unit> Units => _units.AsReadOnly();

    public bool HasFreeSlots => MaxSize > _units.Count;
    public int Size => _units.Count;

    private Army(string id, FactionType faction, Coordinates coordinates, int maxMovementPoints, int maxSize)
    {
        Id = id;
        Faction = faction;
        Coordinates = coordinates;
        MovementPoints = maxMovementPoints;
        MaxMovementPoints = maxMovementPoints;
        MaxSize = maxSize;
    }

    private Army() : this(null!, default, default, default, default) { }

    public static Army Create(
        FactionType faction,
        Coordinates cityCoordinates,
        int maxMovementPoints,
        int maxSize,
        Unit general
    )
    {
        var id = $"army_{Guid.NewGuid()}";
        var army = new Army(id, faction, cityCoordinates, maxMovementPoints, maxSize);

        army.AssignGeneral(general);

        return army;
    }

    public void RecruitUnit(Unit unit)
    {
        if (!HasFreeSlots) throw new GameRuleException("Нельзя нанять юнита, армия уже укомплектована.");
        if (unit.Faction != Faction) throw new GameRuleException("Нельзя нанять юнита, нанятого чужой фракцией.");

        unit.AssignToArmy(Id);
        _units.Add(unit);
    }

    private void AssignGeneral(Unit general)
    {
        if (general.Faction != Faction) throw new GameRuleException("Нельзя нанять генерала, нанятого чужой фракцией.");

        GeneralId = general.Id;
        _units.Add(general);
        general.AssignToArmy(Id);
    }
}