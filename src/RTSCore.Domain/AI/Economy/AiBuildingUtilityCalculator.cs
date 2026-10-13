using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Buldings;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Domain.AI.Economy;

public static class AiBuildingUtilityCalculator
{
    public static List<AiConstructionOption> OrderConstructionOptions(
        BuildingCatalog buildingCatalog,
        AiPersonality personality,
        IReadOnlyCollection<City> cities
    )
    {
        var constructionOptions = new List<AiConstructionOption>();

        foreach (var city in cities)
        {
            var buildings = city.GetConstructableBuildings(buildingCatalog.Templates);

            foreach (var building in buildings)
            {
                var multiplier = building.ProductionCategory == ProductionCategory.Military
                    ? personality.Building.MilitaryMultiplier
                    : personality.Building.EconomicMultiplier;

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
