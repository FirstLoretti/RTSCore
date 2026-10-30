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
    private readonly UnitCatalog _unitCatalog;
    private readonly FrozenDictionary<AiPersonalityType, ImmutableArray<ProductionOption<UnitType>>> _unitsByPersonality;

    public ImmutableArray<ProductionOption<UnitType>> GetUnits(AiPersonalityType personality) => _unitsByPersonality[personality];

    public AiRecruiter(UnitCatalog unitCatalog, AiCatalog aiCatalog)
    {
        _unitCatalog = unitCatalog;

        var tempDic = new Dictionary<AiPersonalityType, ImmutableArray<ProductionOption<UnitType>>>();

        foreach (var personality in aiCatalog.Personalities)
        {
            var units = new List<ProductionOption<UnitType>>();

            foreach (var template in unitCatalog.Templates)
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

    public void RecruitUnits(
        Faction faction,
        Army army,
        AiPersonalityType personality,
        ImmutableArray<ProductionOption<UnitType>> availableUnits
    )
    {
        var allUnits = _unitsByPersonality[personality];

        foreach (var unit in allUnits)
        {
            var availableUnit = availableUnits.FirstOrDefault(u => u.Type == unit.Type);

            if (availableUnit != default)
            {
                while (faction.Gold >= availableUnit.Cost)
                {
                    faction.SpendGold(availableUnit.Cost);
                    army.RecruitUnit(_unitCatalog.GetUnit(availableUnit.Type));
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