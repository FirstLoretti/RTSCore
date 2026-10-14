using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Armies;

public interface IUnitRecruitmentService
{
    void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city);
    void CancelRecruitUnit(Unit unit, Army army, Faction faction, UnitTemplate template);
}