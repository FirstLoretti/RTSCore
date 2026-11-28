using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;


namespace RTSCore.Domain.AI.Recruitment;

public sealed class AiRecruiter
{
    public ImmutableArray<ProductionOption<UnitType>> GetUnits(AiPersonalityType personality) =>
        _unitsByPersonality[personality];

    private readonly UnitCatalog _unitCatalog;
    private readonly FrozenDictionary<AiPersonalityType, ImmutableArray<ProductionOption<UnitType>>> _unitsByPersonality;
    private readonly AiBudgetWeights _budgetWeights;

    public AiRecruiter(UnitCatalog unitCatalog, AiCatalog aiCatalog, AiBudgetWeights budgetWeights)
    {
        _unitCatalog = unitCatalog;
        _budgetWeights = budgetWeights;

        var tempDic = new Dictionary<AiPersonalityType, ImmutableArray<ProductionOption<UnitType>>>();

        foreach (var personality in aiCatalog.AllPersonalities)
        {
            var units = new List<ProductionOption<UnitType>>();

            foreach (var template in unitCatalog.AllTemplates)
            {
                var unit = EvaluateUnitUtility(template, personality);
                if (unit.AiUtility > 0)
                {
                    units.Add(unit);
                }
            }

            tempDic[personality.Type] = [.. units.OrderByDescending(u => u.AiUtility)];
        }

        _unitsByPersonality = tempDic.ToFrozenDictionary();
    }

    public void StartRecruitment(
        Faction faction,
        AiPersonalityType personalityType,
        IReadOnlyCollection<Army> armies,
        IReadOnlyCollection<ProductionOption<UnitType>> availableUnits
    )
    {
        var orderedUnits = _unitsByPersonality[personalityType];
        var availableGold = (int)(faction.Gold * _budgetWeights.Recruitment);

        foreach (var unit in orderedUnits)
        {
            if (availableGold < unit.Cost) continue;

            var availableUnit = availableUnits.FirstOrDefault(u => u.Type == unit.Type);

            if (availableUnit != default)
            {
                foreach (var army in armies)
                {
                    while (availableGold >= availableUnit.Cost)
                    {
                        if (army.RecruitUnit(_unitCatalog.GetTemplate(availableUnit.Type)).IsSuccess)
                        {
                            faction.SpendGold(availableUnit.Cost);
                            availableGold -= availableUnit.Cost;
                        }
                        else break;
                    }
                }
            }
        }
    }

    private static ProductionOption<UnitType> EvaluateUnitUtility(UnitTemplate unit, AiPersonality personality)
    {
        float multiplier = unit.Category switch
        {
            UnitCategory.Infantry => personality.Unit.InfantryMultiplier,
            UnitCategory.RangeInfantry => personality.Unit.RangeInfantryMultiplier,
            _ => throw new SwitchExpressionException($"Unhandled unit category {unit.Category}")
        };

        var utility = (int)(unit.AiUtility * multiplier);

        return new ProductionOption<UnitType>(
            unit.DisplayName, unit.Type, unit.Cost, unit.TurnsToRecruit, unit.ProductionCategory, utility);
    }
}