using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Building
{
    public BuildingId Id { get; init; }
    public BuildingType Type { get; init; }
    public BuildingCategory Category { get; init; }
    public FactionType OwnerFaction { get; init; }
    public CityId CityId { get; init; }
    public bool IsConstructed { get; private set; }
    public bool InConstructProcess { get; private set; }
    public int TurnsToConstruct { get; private set; }
    public int AiUntility { get; private set; }

    public event Action<Building>? OnBuildingCompleted;

    public Building(BuildingId id, BuildingType type, FactionType ownerFaction, CityId cityId)
    {
        Id = id;
        Type = type;
        OwnerFaction = ownerFaction;
        CityId = cityId;

        var template = GameBalance.Buildings.GetTemplate(type);
        TurnsToConstruct = template.TurnsToConstruct;
        Category = template.Category;
        AiUntility = template.AiUtility;
    }

    private Building() { }

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

    public void StartConstruct() => InConstructProcess = true;

    public void AdvanceConstruction()
    {
        if (IsConstructed || !InConstructProcess) return;

        int.Clamp(TurnsToConstruct--, 0, TurnsToConstruct);

        if (TurnsToConstruct == 0)
        {
            IsConstructed = true;
            InConstructProcess = false;
            OnBuildingCompleted?.Invoke(this);
        }
    }
}