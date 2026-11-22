using System.Numerics;

using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies.Combat;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Entities.Armies;

public sealed class Army : AggregateRoot
{
    public CampaignId CampaignId { get; }
    public ArmyId Id { get; }
    public FactionType Faction { get; }

    public UnitId GeneralId { get; private set; }
    public Vector2 Coordinates { get; private set; }
    public int MovementPoints { get; private set; }

    private readonly List<Unit> _units = [];
    public IReadOnlyList<Unit> Units => _units;

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

    internal static Army Create(
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

    private Army() { }

    public IReadOnlyList<Unit> GetReadyCombatants() => [.. _units.Where(u => u.IsAlive && u.IsRecruited)];

    public void RestoreMovementPoints(MovementConfiguration config) => MovementPoints = config.MaxMovementPoints;

    public Result MoveTo(Vector2 destination, int movementCost)
    {
        if (MovementPoints < movementCost) return GameErrors.NotEnoughMovementPoints;
        if (GeneralId == UnitId.Empty) return GameErrors.CantMoveWithoutGeneral;

        MovementPoints -= movementCost;
        Coordinates = destination;

        return Result.Success();
    }

    public Result<UnitId> RecruitUnit(UnitTemplate template)
    {
        if (!HasFreeSlots) return GameErrors.ArmyDoesNotHaveFreeSlots;

        var unit = Unit.CreateTraining(CampaignId, Faction, template, Id);
        _units.Add(unit);

        return unit.Id;
    }

    public Result CancelRecruitment(UnitId id)
    {
        var unit = _units.FirstOrDefault(u => u.Id == id);
        if (unit == null) return GameErrors.UnitNotInTargetArmy(unitId: id, armyId: Id);

        if (!unit.IsAlive || unit.IsRecruited)
            return GameErrors.CantCancelRecruitmentUnitUnsuitableCondition(unit.IsAlive, unit.IsRecruited);

        _units.Remove(unit);

        AddDomainEvent(new CancelRecruitmentEvent(CampaignId, Faction, unit.Cost));

        return Result.Success();
    }

    public Result<UnitId> AddReadyUnit(UnitTemplate template)
    {
        if (!HasFreeSlots) return GameErrors.ArmyDoesNotHaveFreeSlots;

        var unit = Unit.CreateReady(CampaignId, Faction, template, Id);
        _units.Add(unit);

        return unit.Id;
    }

    public Result ReplaceGeneral(UnitTemplate template)
    {
        if (GeneralId != UnitId.Empty)
        {
            var general = _units.FirstOrDefault(u => u.Id == GeneralId);
            if (general == null) return GameErrors.UnitNotInTargetArmy(GeneralId, Id);

            _units.Remove(general);
        }

        var unit = Unit.CreateReady(CampaignId, Faction, template, Id);
        _units.Add(unit);
        GeneralId = unit.Id;

        return Result.Success();
    }

    public Result DisbandUnit(UnitId id)
    {
        if (id == GeneralId) return GameErrors.CantDisbandGeneral;

        var unit = _units.FirstOrDefault(u => u.Id == id);
        if (unit == null) return GameErrors.UnitNotInTargetArmy(unitId: id, armyId: Id);

        _units.Remove(unit);

        return Result.Success();
    }

    public void PurgeDeadUnits()
    {
        if (GeneralId != UnitId.Empty)
        {
            var general = _units.First(u => u.Id == GeneralId);
            if (!general.IsAlive)
            {
                AddDomainEvent(new GeneralDiedEvent(CampaignId, GeneralId));
                GeneralId = UnitId.Empty;
            }
        }

        _units.RemoveAll(u => !u.IsAlive);
    }

    public void TakeCasualties(IReadOnlyCollection<UnitBattleLog> logs)
    {
        foreach (var log in logs)
        {
            var unit = _units.FirstOrDefault(u => u.Id == log.UnitId);
            if (unit == null) continue;

            unit.TakeDamage(log.DamageTaken);
        }

        PurgeDeadUnits();
    }

    public void TurnEnd()
    {
        foreach (var unit in _units)
        {
            if (!unit.IsRecruited)
                unit.AdvanceRecruitment();
        }
    }
}