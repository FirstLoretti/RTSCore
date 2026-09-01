using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Building
{
    public BuildingId Id { get; init; }
    public BuildingType Type { get; init; }
    public FactionType OwnerFaction { get; init; }
    public CityId CityId { get; init; }
    public bool IsConstructed { get; private set; }
    public int TurnsToConstruct { get; private set; }

    public Building(BuildingId id, BuildingType type, FactionType ownerFaction, CityId cityId)
    {
        Id = id;
        Type = type;
        OwnerFaction = ownerFaction;
        CityId = cityId;

        var template = GameBalance.Buildings.GetTemplate(type);
        TurnsToConstruct = template.TurnsToConstruct;
    }

    protected Building() { }

    private Building(
        BuildingId id,
        BuildingType type,
        FactionType ownerFaction,
        CityId cityId,
        bool isConstructed,
        int turnsToConstruct
    )
    {
        Id = id;
        Type = type;
        OwnerFaction = ownerFaction;
        CityId = cityId;
        IsConstructed = isConstructed;
        TurnsToConstruct = turnsToConstruct;
    }

    public static Building CreateWithCustomStatus(
        BuildingId id,
        BuildingType type,
        FactionType ownerFaction,
        CityId cityId,
        bool isConstructed,
        int turnsToConstruct
    )
    {
        return new Building(id, type, ownerFaction, cityId, isConstructed, turnsToConstruct);
    }

    public void AdvanceConstruction()
    {
        if (IsConstructed) return;

        TurnsToConstruct--;
        if (TurnsToConstruct <= 0)
        {
            IsConstructed = true;
        }
    }
}