using System.Numerics;

using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services.Armies;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.ValueObjects.Results;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Entities.Campaign;

public sealed class Army : AggregateRoot
{
    public CampaignId CampaignId { get; }

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

    private Army(
        CampaignId campaignId,
        ArmyId id,
        FactionType faction,
        Vector2 coordinates,
        UnitId generalId
    )
    {
        CampaignId = campaignId;
        Id = id;
        Faction = faction;
        Coordinates = coordinates;
        GeneralId = generalId;

    }

    public static Army Create(
        CampaignId campaignId,
        FactionType faction,
        Vector2 coordinates,
        UnitTemplate general
    )
    {
        var armyId = ArmyId.New();
        var unit = Unit.CreateReady(campaignId, faction, general, armyId);
        var army = new Army(campaignId, armyId, faction, coordinates, unit.Id);

        army._units.Add(unit);

        return army;
    }

    public static Army CreateWithMovementPoints(
        CampaignId campaignId,
        FactionType faction,
        Vector2 coordinates,
        UnitTemplate general,
        MovementConfiguration configuration
    )
    {
        var army = Create(campaignId, faction, coordinates, general);
        army.RestoreMovementPoints(configuration);

        return army;
    }

    public void RestoreMovementPoints(MovementConfiguration configuration)
        => MovementPoints = configuration.MaxMovementPoints;

    public void MoveTo(Vector2 destination, int movementCost)
    {
        var absCost = int.Abs(movementCost);
        if (MovementPoints < absCost) throw new GameRuleException("Недостаточно очков перемещения.");

        MovementPoints -= absCost;
        Coordinates = destination;
    }

    internal void RecruitUnit(UnitTemplate template)
    {
        if (!HasFreeSlots)
            throw new GameRuleException("Нельзя нанять юнита, армия уже укомплектована.");

        _units.Add(Unit.CreateTraining(CampaignId, Faction, template, Id));
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

    internal void PurgeDeadUnits()
    {
        var deadUnits = _units.Where(u => !u.IsAlive).ToList();
        foreach (var unit in deadUnits)
        {
            _units.Remove(unit);
        }
    }

    internal void TakeCasualties(List<BattleLog> logs)
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

    public void Attack(Army enemy, MovementConfiguration configuration)
    {
        if (_units.Count == 0)
            throw new GameRuleException("Пустая армия не может атаковать");

        if (enemy.Faction == Faction)
            throw new GameRuleException("Нельзя атаковать армию своей фракции");

        var movementCost = ArmyMovementCalculator.CalculateCost(
            Coordinates,
            enemy.Coordinates,
            configuration.BaseUnitDistanceCost
        );

        MoveTo(enemy.Coordinates, movementCost);

        Target = enemy.Id;
        enemy.Target = Id;
    }
}