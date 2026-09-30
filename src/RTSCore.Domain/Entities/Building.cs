using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Building
{
    public BuildingId Id { get; }
    public BuildingType Type { get; }
    public FactionType Faction { get; }
    public CityId CityId { get; }
    public int Cost { get; }

    public bool IsConstructed { get; private set; }
    public bool InConstructProcess { get; private set; }
    public int TurnsToConstruct { get; private set; }

    private static BuildingId GenerateId() => $"building_{Guid.NewGuid():N}";

    private Building(
        BuildingId id,
        BuildingType type,
        FactionType faction,
        CityId cityId,
        int turnsToConstruct,
        int cost
    )
    {
        Id = id;
        Type = type;
        Faction = faction;
        CityId = cityId;
        TurnsToConstruct = turnsToConstruct;
        Cost = cost;
    }

    private Building() { }

    internal static Building CreateUnderConstruction(
        BuildingTemplate template,
        FactionType faction,
        CityId cityId
    ) => new(GenerateId(), template.Type, faction, cityId, template.TurnsToConstruct, template.Cost);

    internal static Building CreateConstructed(
        BuildingType type,
        FactionType faction,
        CityId cityId
    ) => new(GenerateId(), type, faction, cityId, turnsToConstruct: 0, cost: 0);

    public void StartConstruct()
    {
        if (IsConstructed) throw new GameRuleException("Нельзя начать строительство, здание уже построено");
        InConstructProcess = true;
    }

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