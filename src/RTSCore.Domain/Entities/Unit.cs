using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Unit
{
    public UnitId Id { get; init; }
    public UnitType Type { get; init; }
    public FactionType Faction { get; init; }

    public ArmyId ArmyId { get; private set; }
    public int Health { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }
    public int TurnsToRecruit { get; private set; }
    public bool IsAlive => IsRecruited && Health > 0;
    public bool IsRecruited => TurnsToRecruit <= 0;

    private Unit(
        UnitId id,
        FactionType faction,
        UnitTemplate template,
        ArmyId armyId,
        int turnsToRecruit)
    {
        Id = id;
        Faction = faction;
        ArmyId = armyId;
        Type = template.Type;
        Health = template.MaxHealth;
        TurnsToRecruit = turnsToRecruit;
    }

    internal static Unit CreateReady(FactionType faction, UnitTemplate template, ArmyId armyId)
    {
        var id = $"unit_{Guid.NewGuid():N}";
        return new Unit(id, faction, template, armyId, turnsToRecruit: 0);
    }

    internal static Unit CreateTraining(FactionType faction, UnitTemplate template, ArmyId armyId)
    {
        var id = $"unit_{Guid.NewGuid():N}";
        return new Unit(id, faction, template, armyId, template.TurnsToRecruit);
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        Health = int.Max(0, Health - int.Max(0, amount));
    }

    public void AdvanceRecruitment()
    {
        if (IsRecruited || !IsAlive)
            throw new ArgumentException("Невозможно продвинуть найм нанятого или мёртвого отряда");

        TurnsToRecruit--;
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
            Experience = 0;
    }
}