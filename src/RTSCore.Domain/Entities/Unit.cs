using RTSCore.Domain.ValueObjects;

using static RTSCore.Domain.Services.GameBalance;

namespace RTSCore.Domain.Entities;

public class Unit
{
    public UnitId Id { get; init; }
    public UnitType Type { get; init; }
    public FactionType OwnerFaction { get; init; }
    public CityId? CurrentCityId { get; private set; }
    public int Health { get; private set; }
    public int Damage { get; private set; }
    public int Armor { get; private set; }
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public int TurnsToRecruit { get; private set; }
    public bool IsAlive => IsRecruited && Health > 0;
    public bool IsRecruited => TurnsToRecruit <= 0;

    public Unit(UnitId id, FactionType ownerFaction, UnitTemplate template, CityId? currentCityId = null)
    {
        Id = id;
        OwnerFaction = ownerFaction;
        CurrentCityId = currentCityId;

        Type = template.Type;
        Health = template.MaxHealth;
        Damage = template.Damage;
        Armor = template.Armor;
        TurnsToRecruit = template.TurnsToRecruit;
    }

    private Unit() { }

    private Unit(UnitId id, FactionType ownerFaction, UnitTemplate template, int turnsToRecruit, CityId? currentCityId = null)
    {
        Id = id;
        OwnerFaction = ownerFaction;
        CurrentCityId = currentCityId;

        Type = template.Type;
        Health = template.MaxHealth;
        Damage = template.Damage;
        Armor = template.Armor;
        TurnsToRecruit = turnsToRecruit;

    }

    public static Unit CreateWithCustomStatus(
        UnitId id, FactionType ownerFaction, UnitTemplate template, int turnsToRecruit, CityId? currentCityId = null)
    {
        return new Unit(id, ownerFaction, template, turnsToRecruit, currentCityId);
    }

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

        Health = Units.CalculateStat(template.MaxHealth, template.HealthGrowthRate, Level);
        Damage = Units.CalculateStat(template.Damage, template.DamageGrowthRate, Level);
    }
}