using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Building
{
    public BuildingId Id { get; init; }
    public BuildingType Type { get; init; }
    public FactionType Faction { get; init; }
    public CityId CityId { get; init; }
    public bool IsConstructed { get; private set; }
    public bool InConstructProcess { get; private set; }
    public int TurnsToConstruct { get; private set; }

    private Building(
        BuildingId id,
        BuildingType type,
        FactionType faction,
        CityId cityId,
        int turnsToConstruct
    )
    {
        Id = id;
        Type = type;
        Faction = faction;
        CityId = cityId;
        TurnsToConstruct = turnsToConstruct;
    }

    private Building() { }

    internal static Building Create(
        BuildingTemplate template,
        BuildingType type,
        FactionType faction,
        CityId cityId
    )
    {
        var id = $"building_{Guid.NewGuid():N}";
        return new(id, type, faction, cityId, template.TurnsToConstruct);
    }

    internal static Building CreateConstructed(
        BuildingType type,
        FactionType faction,
        CityId cityId
    )
    {
        var id = $"building_{Guid.NewGuid():N}";
        return new(id, type, faction, cityId, turnsToConstruct: 0);
    }

    public void StartConstruct() => InConstructProcess = true;

    public void AdvanceConstruction()
    {
        if (InConstructProcess)
        {
            TurnsToConstruct--;

            if (TurnsToConstruct == 0)
            {
                IsConstructed = true;
                InConstructProcess = false;
            }
        }
    }
}