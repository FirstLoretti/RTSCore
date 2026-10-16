using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Campaign;

namespace RTSCore.Domain.Entities.Armies;

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

    public bool IsAlive => Health > 0;
    public bool IsRecruited => TurnsToRecruit == 0;

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

    internal void TakeDamage(int amount) => Health = int.Max(0, Health - amount);

    internal void AdvanceRecruitment() => TurnsToRecruit = int.Max(0, TurnsToRecruit - 1);

    internal void AddExperience(int amount, IReadOnlyList<int> expToLevel)
    {
        if (Level == expToLevel.Count - 1) return;

        Experience += amount;

        while (Level < expToLevel.Count - 1 && Experience >= expToLevel[Level + 1])
        {
            Level++;
        }
    }
}