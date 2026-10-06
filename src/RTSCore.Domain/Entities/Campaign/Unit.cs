using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Entities.Campaign;

public sealed class Unit
{
    public CampaignId CampaignId { get; }

    public UnitId Id { get; }
    public UnitType Type { get; }
    public FactionType Faction { get; }
    public ArmyId ArmyId { get; private set; }

    public int Health { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }
    public int TurnsToRecruit { get; private set; }

    public bool IsAlive => IsRecruited && Health > 0;
    public bool IsRecruited => TurnsToRecruit <= 0;

    private Unit(
        CampaignId campaignId,
        UnitId id,
        FactionType faction,
        UnitTemplate template,
        ArmyId armyId,
        int turnsToRecruit
    )
    {
        CampaignId = campaignId;
        Id = id;
        Faction = faction;
        ArmyId = armyId;
        Type = template.Type;
        Health = template.MaxHealth;
        TurnsToRecruit = turnsToRecruit;
    }

    internal static Unit CreateReady(
        CampaignId campaignId,
        FactionType faction,
        UnitTemplate template,
        ArmyId armyId
    ) => new(campaignId, UnitId.New(), faction, template, armyId, turnsToRecruit: 0);

    internal static Unit CreateTraining(
        CampaignId campaignId,
        FactionType faction,
        UnitTemplate template,
        ArmyId armyId
    ) => new(campaignId, UnitId.New(), faction, template, armyId, template.TurnsToRecruit);

    internal void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        Health = int.Max(0, Health - int.Max(0, amount));
    }

    internal void AdvanceRecruitment()
    {
        if (IsRecruited || !IsAlive)
            throw new ArgumentException("Невозможно продвинуть найм нанятого или мёртвого отряда");

        TurnsToRecruit--;
    }

    internal void AddExperience(int amount, IReadOnlyList<int> expToNextLevel)
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