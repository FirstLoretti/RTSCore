using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Barrack : Building
{
    public int ActiveRecruitmentSlots { get; private set; }
    public int MaxRecruitmentSlots { get; init; }
    public bool HasFreeRecruitmentSlots => ActiveRecruitmentSlots < MaxRecruitmentSlots;

    public Barrack(BuildingId id, BuildingTemplate template)
        : base(id, template.Type, template.Faction)
    {
        MaxRecruitmentSlots = template.MaxRecruitmentSlots;
    }

    protected Barrack(BuildingId id, BuildingType type, FactionType faction)
        : base(id, type, faction) { }

    public void AddUnitToQueue()
    {
        ActiveRecruitmentSlots++;
    }
}