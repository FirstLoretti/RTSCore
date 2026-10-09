using RTSCore.Domain.AI.ValueObjects;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.AI.Services;

public class AiBuildingUtilityCalculator(
    BuildingConfiguration buildingConfiguration,
    AiPersonality personality
)
{
    public List<AiConstructionOption> GetOrderedConstructionOptions(IReadOnlyCollection<City> cities)
    {
        var constructionOptions = new List<AiConstructionOption>();

        foreach (var city in cities)
        {
            var buildings = city.GetConstructableBuildings(buildingConfiguration.Templates);

            foreach (var building in buildings)
            {
                var multiplier = building.ProductionCategory == ProductionCategory.Military
                    ? personality.BuildingWeights.MilitaryMultiplier
                    : personality.BuildingWeights.EconomicMultiplier;

                var utility = (int)(building.AiUtility * multiplier);

                if (utility > 0)
                {
                    var constructOption = new AiConstructionOption(city.Id, building.Type, building.Cost, utility);
                    constructionOptions.Add(constructOption);
                }
            }
        }

        return [.. constructionOptions.OrderByDescending(o => o.Utility)];
    }
}
