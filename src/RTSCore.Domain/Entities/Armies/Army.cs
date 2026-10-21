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
    public IReadOnlyCollection<Unit> Units => _units;

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

    public IReadOnlyCollection<Unit> GetReadyCombatants() => [.. _units.Where(u => u.IsAlive && u.IsRecruited)];

    public void RestoreMovementPoints(MovementConfiguration config) => MovementPoints = config.MaxMovementPoints;

    public Result MoveTo(Vector2 destination, int movementCost)
    {
        if (MovementPoints < movementCost) return Result.Failure(GameErrors.NotEnoughMovementPoints);

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

    public Result<UnitId> AddReadyUnit(UnitTemplate template)
    {
        if (!HasFreeSlots) return GameErrors.ArmyDoesNotHaveFreeSlots;

        var unit = Unit.CreateReady(CampaignId, Faction, template, Id);
        _units.Add(unit);

        return unit.Id;
    }

    public void ReplaceGeneral(UnitTemplate template)
    {
        if (GeneralId != UnitId.Empty)
        {
            var general = GetUnitOrThrow(GeneralId);
            _units.Remove(general);
        }

        var unit = Unit.CreateReady(CampaignId, Faction, template, Id);
        _units.Add(unit);
        GeneralId = unit.Id;
    }

    public Result DisbandUnit(UnitId id)
    {
        if (id == GeneralId) return GameErrors.CantDisbandGeneral;

        var unit = GetUnitOrThrow(id);
        _units.Remove(unit);

        return Result.Success();
    }

    public void PurgeDeadUnits()
    {
        var general = GetUnitOrThrow(GeneralId);

        if (!general.IsAlive)
        {
            AddDomainEvent(new RemoveGeneralEvent(CampaignId, GeneralId));
            GeneralId = UnitId.Empty;
        }

        _units.RemoveAll(u => !u.IsAlive);
    }

    public void TakeCasualties(List<UnitBattleLog> logs)
    {
        foreach (var log in logs)
        {
            var unit = GetUnitOrThrow(log.UnitId);
            unit.TakeDamage(log.DamageTaken);
        }

        PurgeDeadUnits();
    }

    private Unit GetUnitOrThrow(UnitId id) => _units.SingleOrDefault(u => u.Id == id)
        ?? throw new InvalidOperationException($"Army:{Id} does not contain it's own unit:{id}");

}