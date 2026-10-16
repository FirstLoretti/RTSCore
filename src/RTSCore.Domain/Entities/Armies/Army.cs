using System.Numerics;

using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies.Combat;
using RTSCore.Domain.Entities.Campaign;
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

    public void RestoreMovementPoints(MovementConfiguration config) => MovementPoints = config.MaxMovementPoints;

    public Result MoveTo(Vector2 destination, int movementCost)
    {
        if (MovementPoints < movementCost) return Result.Failure(GameErrors.NotEnoughMovementPoints);

        MovementPoints -= movementCost;
        Coordinates = destination;

        return Result.Success();
    }

    public Result RecruitUnit(UnitTemplate template)
    {
        if (!HasFreeSlots) return Result.Failure(GameErrors.ArmyDontHaveFreeSlots);

        _units.Add(Unit.CreateTraining(CampaignId, Faction, template, Id));

        return Result.Success();
    }

    public void AssignGeneral(UnitTemplate template)
    {
        if (GeneralId != UnitId.Empty)
        {
            var general = _units.Single(u => u.Id == GeneralId);
            DisbandUnit(general);
        }

        var unit = Unit.CreateReady(CampaignId, Faction, template, Id);
        _units.Add(unit);
        GeneralId = unit.Id;
    }

    public void DisbandUnit(Unit unit)
    {
        _units.Remove(unit);

        if (unit.Id == GeneralId)
        {
            GeneralId = UnitId.Empty;
            // TODO: Event
        }
    }

    public void PurgeDeadUnits()
    {
        var general = _units.Single(u => u.Id == GeneralId);

        if (!general.IsAlive)
        {
            GeneralId = UnitId.Empty;
            // TODO: Event
        }

        _units.RemoveAll(u => !u.IsAlive);
    }

    public void TakeCasualties(List<UnitBattleLog> logs)
    {
        foreach (var log in logs)
        {
            var unit = _units.Single(u => u.Id == log.UnitId);

            unit.TakeDamage(log.DamageTaken);
        }

        PurgeDeadUnits();
    }
}