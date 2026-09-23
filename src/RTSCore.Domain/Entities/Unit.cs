using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Unit
{
    public UnitId Id { get; init; }
    public UnitType Type { get; init; }
    public FactionType Faction { get; init; }

    public string ArmyId { get; private set; } = string.Empty;
    public int Health { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }
    public int TurnsToRecruit { get; private set; }
    public bool IsAlive => IsRecruited && Health > 0;
    public bool IsRecruited => TurnsToRecruit <= 0;

    internal Unit(FactionType faction, UnitTemplate template, string armyId)
    {
        Id = $"unit_{Guid.NewGuid()}";
        Faction = faction;
        ArmyId = armyId;

        Type = template.Type;
        Health = template.MaxHealth;
        TurnsToRecruit = template.TurnsToRecruit;
    }

    private Unit() { }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        Health = int.Max(0, Health - int.Max(0, amount));
    }

    public void AddExperience(int amount, IReadOnlyList<int> expToNextLevel)
    {
        if (!IsAlive || Level == expToNextLevel.Count) return;

        Experience += int.Max(0, amount);

        while (Level < expToNextLevel.Count && Experience >= expToNextLevel[Level])
        {
            Experience -= expToNextLevel[Level];
            Level++;
        }

        if (Level == expToNextLevel.Count)
        {
            Experience = 0;
        }
    }
}