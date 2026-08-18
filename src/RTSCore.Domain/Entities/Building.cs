using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Building
{
    public BuildingId Id { get; init; }
    public BuildingType Type { get; init; }
    public FactionType Faction { get; init; }
    public int Health { get; init; }
    public int Level { get; init; } = 1;
    public bool IsAlive => Health > 0;

    public Building(BuildingId id, BuildingType type, FactionType faction)
    {
        Id = id;
        Type = type;
        Faction = faction;

        var template = GameBalance.Buildings.GetTemplate(type);
        Health = template.MaxHealth;
    }
}