namespace RTSCore.Domain.AI.Recruitment;

using System.Runtime.CompilerServices;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;

public static class AiRecruitmentPlanner
{
    public static IReadOnlyCollection<AiRecruitOption> PlanRecruitment(IEnumerable<AiRecruitOption> units, int gold)
    {
        var unitsToRecruitment = new List<AiRecruitOption>();

        foreach (var unit in units)
        {
            if (gold >= unit.Cost)
            {
                gold -= unit.Cost;
                unitsToRecruitment.Add(unit);
            }
        }

        return unitsToRecruitment;
    }

    public static AiRecruitOption EvaluateUnitUtility(UnitTemplate unit, AiPersonality personality)
    {
        float multiplier = unit.Category switch
        {
            UnitCategory.Infantry => personality.Unit.InfantryMultiplier,
            UnitCategory.RangeInfantry => personality.Unit.RangeInfantryMultiplier,
            _ => throw new SwitchExpressionException($"Unhandled unit category {unit.Category}")
        };

        var utility = (int)(unit.AiUtility * multiplier);

        return new AiRecruitOption(unit.Type, unit.Cost, utility);
    }
}