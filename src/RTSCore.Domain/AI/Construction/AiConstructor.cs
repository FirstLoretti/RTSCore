using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.AI.Construction;

public sealed class AiConstructor
{
    private readonly AiBudgetWeights _budgetWeights;
    private readonly BuildingCatalog _buildingCatalog;
    private readonly FrozenDictionary<AiPersonalityType, ImmutableArray<ProductionOption<BuildingType>>> _buildingsByPersonality;

    private readonly int _minBuildingCost;

    public AiConstructor(AiCatalog aiCatalog, BuildingCatalog buildingCatalog, AiBudgetWeights budgetWeights)
    {
        _budgetWeights = budgetWeights;
        _buildingCatalog = buildingCatalog;
        _minBuildingCost = buildingCatalog.AllTemplates.Min(b => b.Cost);

        var tempDic = new Dictionary<AiPersonalityType, ImmutableArray<ProductionOption<BuildingType>>>(
            aiCatalog.AllPersonalities.Length
        );

        foreach (var personality in aiCatalog.AllPersonalities)
        {
            tempDic[personality.Type] = EvaluateBuildings(_buildingCatalog, personality);
        }

        _buildingsByPersonality = tempDic.ToFrozenDictionary();
    }

    public void StartConstruction(Faction faction, AiPersonalityType personalityType, IReadOnlyCollection<City> cities)
    {
        var availableGold = (int)(faction.Gold * _budgetWeights.Construction);
        if (availableGold < _minBuildingCost || cities.Count == 0) return;

        var orderedBuildings = _buildingsByPersonality[personalityType];

        foreach (var building in orderedBuildings)
        {
            if (availableGold < building.Cost) continue;

            var template = _buildingCatalog.GetTemplate(building.Type);

            foreach (var city in cities)
            {
                if (availableGold < building.Cost) continue;

                if (city.StartConstruction(template).IsSuccess)
                {
                    faction.SpendGold(building.Cost);
                    availableGold -= building.Cost;
                }
            }
        }
    }

    private static ImmutableArray<ProductionOption<BuildingType>> EvaluateBuildings(
        BuildingCatalog catalog,
        AiPersonality personality
    )
    {
        var options = new List<ProductionOption<BuildingType>>();

        foreach (var building in catalog.AllTemplates)
        {
            var multiplier = building.ProductionCategory == ProductionCategory.Military
                ? personality.Building.MilitaryMultiplier
                : personality.Building.EconomicMultiplier;

            var utility = (int)(building.AiUtility * multiplier);

            if (utility > 0)
            {
                var option = new ProductionOption<BuildingType>(
                    building.DisplayName,
                    building.Type,
                    building.Cost,
                    building.TurnsToConstruct,
                    building.ProductionCategory,
                    utility
                );

                options.Add(option);
            }
        }

        return [.. options.OrderByDescending(o => o.AiUtility)];
    }
}