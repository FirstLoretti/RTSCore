using RTSCore.Domain.ValueObjects;

using static RTSCore.Domain.Services.GameBalance;

namespace RTSCore.Domain.Entities;

public class Unit(
    UnitId id,
    UnitType type,
    UnitTemplate template,
    FactionType faction
)
{
    public UnitId Id { get; init; } = id;
    public UnitType Type { get; init; } = type;
    public FactionType Faction { get; init; } = faction;
    public int Health { get; private set; } = template.MaxHealth;
    public int Damage { get; private set; } = template.Damage;
    public int Armor { get; private set; } = template.Armor;
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public bool IsAlive => Health > 0;

    protected Unit() : this(default, default, default, default) { }

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
        Health =
            Units.CalculateStat(template.MaxHealth, template.HealthGrowthRate, Level);
        Damage =
            Units.CalculateStat(template.Damage, template.DamageGrowthRate, Level);
    }
}