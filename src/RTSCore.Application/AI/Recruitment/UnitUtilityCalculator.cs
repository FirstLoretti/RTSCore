namespace RTSCore.Application.AI.Brains;

using RTSCore.Application.AI.Recruitment;
using RTSCore.Domain.AI.ValueObjects;
using RTSCore.Domain.Common;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Templates;

public class UnitUtilityCalculator
{
    public IReadOnlyCollection<AiRecruitOption> CalculateFor(
        IReadOnlyCollection<UnitTemplate> units,
        AiPersonality personality)
    {
        ArgumentNullException.ThrowIfNull(units);

        return units.Count == 0
            ? []
            : [.. units
                .Select(u => GetRecruitOption(u,personality))
                .Where(o => o.Utility > 0)
                .OrderByDescending(o => o.Utility)];
    }

    private static AiRecruitOption GetRecruitOption(UnitTemplate unit, AiPersonality personality)
    {
        float multiplier = unit.Category switch
        {
            UnitCategory.Infantry => personality.UnitWeights.InfantryMultiplier,
            UnitCategory.RangeInfantry => personality.UnitWeights.RangeInfantryMultiplier,
            _ => throw ThrowHelper.UnsupportedCategory(unit.Category)
        };

        var utility = (int)(unit.AiUtility * multiplier);

        return new AiRecruitOption(unit.Type, unit.Cost, utility);
    }
}