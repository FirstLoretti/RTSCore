using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public class UnitRecruitmentService
{
    public void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city)
    {
        faction.SpendGold(unit.Cost);

        if (city != null)
        {
            city.RecruitUnit(army, unit);
        }
        else
        {
            army.RecruitUnit(unit);
        }
    }

    public void CancelRecruitUnit(Unit unit, Army army, Faction faction, UnitTemplate template)
    {
        faction.EarnGold(template.Cost);
        army.CancelRecruitUnit(unit);
    }
}