using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;


namespace RTSCore.Domain.AI.Recruitment;

public sealed class AiRecruiter
{
    private readonly UnitCatalog _unitCatalog;
    private readonly FrozenDictionary<AiPersonality, ImmutableArray<AiRecruitOption>> _evaluatedOptions;

    public AiRecruiter(UnitCatalog unitCatalog, AiCatalog aiCatalog)
    {
        _unitCatalog = unitCatalog;

        var temp = new Dictionary<AiPersonality, ImmutableArray<AiRecruitOption>>();

        foreach (var personality in aiCatalog.Personalities)
        {
            var options = new List<AiRecruitOption>();

            foreach (var template in unitCatalog.Templates)
            {
                var option = AiRecruitmentPlanner.EvaluateUnitUtility(template, personality);
                if (option.Utility > 0)
                {
                    options.Add(option);
                }
            }

            temp[personality] = [.. options.OrderByDescending(o => o.Utility)];
        }

        _evaluatedOptions = temp.ToFrozenDictionary();
    }

    public void RecruitUnits(Faction faction, Army army, AiPersonality personality, City city)
    {
        HashSet<UnitType> units = [.. city.GetRecruitableUnits(_unitCatalog.Templates).Select(u => u.Type)];
        var options = _evaluatedOptions[personality]
            .Where(o => units.Contains(o.UnitType))
            .ToList();

        var unitsToRecruitment = AiRecruitmentPlanner.PlanRecruitment(options, faction.Gold);

        foreach (var unit in unitsToRecruitment)
        {
            faction.SpendGold(unit.Cost);
            army.RecruitUnit(_unitCatalog.GetUnit(unit.UnitType));
        }
    }
}