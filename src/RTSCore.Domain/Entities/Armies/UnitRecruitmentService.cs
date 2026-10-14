using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Armies;

public class UnitRecruitmentService : IUnitRecruitmentService
{
    public void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city)
    {
        if (army.Faction != faction.Type)
            throw new NullReferenceException("Нельзя нанять юнита в армию чужой фракции");

        if (unit.RequiredBuilding is not null)
        {
            if (city is null)
                throw new NullReferenceException("Для найма регулярного юнита требуется передать город");

            if (city.Faction != city.Faction)
                throw new NullReferenceException("Нельзя нанять юнита в чужом городе");

            city.AssertCanRecruit(unit);
        }

        faction.SpendGold(unit.Cost);
        army.RecruitUnit(unit);
    }

    public void CancelRecruitUnit(Unit unit, Army army, Faction faction, UnitTemplate template)
    {
        faction.EarnGold(template.Cost);
        army.CancelRecruitUnit(unit);
    }
}