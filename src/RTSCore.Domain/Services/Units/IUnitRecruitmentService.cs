using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Services.Units;

public interface IUnitRecruitmentService
{
    void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city);
    void CancelRecruitUnit(Unit unit, Army army, Faction faction, UnitTemplate template);
}