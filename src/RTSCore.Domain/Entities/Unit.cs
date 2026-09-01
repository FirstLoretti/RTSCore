using RTSCore.Domain.ValueObjects;

using static RTSCore.Domain.Services.GameBalance;

namespace RTSCore.Domain.Entities;

public class Unit
{
    public UnitId Id { get; init; }
    public UnitType Type { get; init; }
    public FactionType Faction { get; init; }
    public CityId? CurrentCityId { get; private set; }
    public int Health { get; private set; }
    public int Damage { get; private set; }
    public int Armor { get; private set; }
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public bool IsAlive => Health > 0;

    public Unit(UnitId id, UnitType type, FactionType faction, CityId? currentCityId = null)
    {
        Id = id;
        Type = type;
        Faction = faction;
        CurrentCityId = currentCityId;

        var template = Units.GetTemplate(type);
        Health = template.MaxHealth;
        Damage = template.Damage;
        Armor = template.Armor;
    }

    protected Unit() { }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        Health = int.Max(0, Health - int.Max(0, amount));
    }

    public void AddExperience(int amount)
    {
        if (!IsAlive || Level == Units.ExpToNextLevel.Length) return;

        Experience += int.Max(0, amount);

        while (
            Level < Units.ExpToNextLevel.Length &&
            Experience >= Units.ExpToNextLevel[Level - 1]
        )
        {
            Experience -= Units.ExpToNextLevel[Level - 1];
            Level++;
            RecalculateStats();
        }

        if (Level == Units.ExpToNextLevel.Length)
        {
            Experience = Units.ExpToNextLevel.Last();
        }
    }

    private void RecalculateStats()
    {
        var template = Units.GetTemplate(Type);

        Health =
            Units.CalculateStat(template.MaxHealth, template.HealthGrowthRate, Level);
        Damage =
            Units.CalculateStat(template.Damage, template.DamageGrowthRate, Level);
    }
}