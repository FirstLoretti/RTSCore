using RTSCore.Domain.Configurations.Units;
using RTSCore.Domain.Entities.Campaign;

namespace RTSCore.Domain.Services.Units;

public interface IUnitRecruitmentService
{
    void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city);
    void CancelRecruitUnit(Unit unit, Army army, Faction faction, UnitTemplate template);
}