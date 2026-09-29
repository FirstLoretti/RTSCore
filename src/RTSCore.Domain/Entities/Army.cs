using System.Numerics;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Domain.Entities;

public class Army
{
    public ArmyId Id { get; }
    public FactionType Faction { get; }
    public UnitId GeneralId { get; private set; }
    public Vector2 Coordinates { get; private set; }

    public int MovementPoints { get; private set; }
    public ArmyId? Target { get; private set; }

    private readonly List<Unit> _units = [];
    public IReadOnlyList<Unit> Units => _units.AsReadOnly();

    public const int MaxSize = 20;
    public bool HasFreeSlots => _units.Count < MaxSize;

    private static ArmyId GenerateId() => $"army_{Guid.NewGuid():N}";

    private Army(
        ArmyId id,
        FactionType faction,
        Vector2 coordinates,
        UnitId generalId
    )
    {
        Id = id;
        Faction = faction;
        Coordinates = coordinates;
        GeneralId = generalId;

    }

    internal static Army Create(FactionType faction, Vector2 coordinates, UnitTemplate general)
    {
        var id = GenerateId();
        var unit = Unit.CreateReady(faction, general, id);
        var army = new Army(id, faction, coordinates, unit.Id);

        army._units.Add(unit);

        return army;
    }

    internal static Army CreateWithMovementPoints(
        FactionType faction,
        Vector2 coordinates,
        UnitTemplate general,
        ArmyConfiguration configuration
    )
    {
        var army = Create(faction, coordinates, general);
        army.RestoreMovementPoints(configuration);

        return army;
    }

    public void MoveTo(Vector2 destination, int movementCost)
    {
        var absCost = int.Abs(movementCost);
        if (MovementPoints < absCost) throw new GameRuleException("Недостаточно очков перемещения.");

        MovementPoints -= absCost;
        Coordinates = destination;
    }

    public void RestoreMovementPoints(ArmyConfiguration configuration)
        => MovementPoints = configuration.MaxMovementPoints;

    internal void RecruitUnit(UnitTemplate template)
    {
        if (!HasFreeSlots) throw new GameRuleException("Нельзя нанять юнита, армия уже укомплектована.");

        _units.Add(Unit.CreateTraining(Faction, template, Id));
    }

    public void CancelRecruitUnit(Unit unit)
    {
        if (unit.IsRecruited)
            throw new GameRuleException("Нельзя отменить найм нанятого юнита");

        if (unit.Faction == Faction)
            throw new GameRuleException("Нельзя распустить юнита другой фракции");

        _units.Remove(unit);
    }

    public void DisbandUnit(Unit unit)
    {
        if (!unit.IsAlive)
            throw new GameRuleException($"Не нанятого или мёртвого юнита {unit.Id} нельзя распустить");

        if (unit.Faction != Faction)
            throw new GameRuleException("Нельзя распустить юнита другой фракции");

        _units.Remove(unit);
    }

    private void PurgeDeadUnits()
    {
        var deadUnits = _units.Where(u => !u.IsAlive).ToList();
        foreach (var unit in deadUnits)
        {
            _units.Remove(unit);
        }
    }

    public void TakeCasualties(List<BattleLog> logs)
    {
        foreach (var log in logs)
        {
            var unit = _units.FirstOrDefault(u => u.Id == log.UnitId)
                ?? throw new NullReferenceException("Batlelog содержит юнита не существуюшего в армии");

            unit.TakeDamage(log.DamageTaken);
        }

        PurgeDeadUnits();
    }

    public void ReleaseFromBattle() => Target = null;

    public void Attack(Army enemy, ArmyConfiguration configuration)
    {
        if (_units.Count == 0)
            throw new GameRuleException("Пустая армия не может атаковать");

        if (enemy.Faction == Faction)
            throw new GameRuleException("Нельзя атаковать армию своей фракции");

        var movementCost = ArmyMovementCalculator.CalculateCost(
            Coordinates,
            enemy.Coordinates,
            configuration.MovementUnitDistanceCost
        );

        MoveTo(enemy.Coordinates, movementCost);

        Target = enemy.Id;
        enemy.Target = Id;
    }
}