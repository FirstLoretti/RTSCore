using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Services.Units;

public class UnitRecruitmentService : IUnitRecruitmentService
{
    public void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city)
    {
        if (army.Faction != faction.Type)
            throw new GameRuleException("Нельзя нанять юнита в армию чужой фракции");

        if (unit.RequiredBuilding is not null)
        {
            if (city is null)
                throw new GameRuleException("Для найма регулярного юнита требуется передать город");

            if (city.Faction != city.Faction)
                throw new GameRuleException("Нельзя нанять юнита в чужом городе");

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