using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IUnitRecruitmentService
{
    void RecruitUnit(UnitTemplate unit, Army army, Faction faction, City? city);
    void CancelRecruitUnit(Unit unit, Army army, Faction faction, UnitTemplate template);
}