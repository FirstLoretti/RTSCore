using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Unit(UnitId id, UnitTemplate template, FactionId factionHolder)
{
    public UnitId Id { get; init; } = id;
    public UnitType Type { get; init; } = template.Type;
    public FactionId FactionHolder { get; init; } = factionHolder;
    public int Health { get; private set; } = template.MaxHealth;
    public int Damage { get; private set; } = template.Damage;
    public int Armor { get; private set; } = template.Armor;
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public bool IsAlive => Health > 0;

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        Health = int.Max(0, Health - int.Abs(amount));
    }

    public void AddExperience(int amount)
    {
        if (!IsAlive || Level == GameBalance.ExpToNextLevel.Length) return;

        Experience += int.Max(0, amount);

        while (
            Level < GameBalance.ExpToNextLevel.Length &&
            Experience >= GameBalance.ExpToNextLevel[Level - 1]
        )
        {
            Experience -= GameBalance.ExpToNextLevel[Level - 1];
            Level++;
            RecalculateStats();
        }

        if (Level == GameBalance.ExpToNextLevel.Length)
        {
            Experience = GameBalance.ExpToNextLevel.Last();
        }
    }

    private void RecalculateStats()
    {
        Health = GameBalance.CalculateStat(template.MaxHealth, template.HealthGrowthRate, Level);
        Damage = GameBalance.CalculateStat(template.Damage, template.DamageGrowthRate, Level);
        Armor = GameBalance.CalculateStat(template.Armor, template.ArmorGrowthRate, Level);
    }
}