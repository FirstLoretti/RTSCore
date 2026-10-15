using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.AI.Construction;

public static class AiConstructionPlanner
{
    public static IReadOnlyCollection<AiConstructionOption> PlaneConstruction(
        BuildingCatalog catalog,
        AiPersonality personality,
        IReadOnlyCollection<City> cities,
        int gold
    )
    {
        var buildings = EvaluateAvailableBuildings(catalog, personality, cities);
        var buildingsToConstruct = new List<AiConstructionOption>();

        foreach (var building in buildings)
        {
            if (gold >= building.Cost)
            {
                buildingsToConstruct.Add(building);
                gold -= building.Cost;
            }
        }

        return buildingsToConstruct;
    }

    private static List<AiConstructionOption> EvaluateAvailableBuildings(
        BuildingCatalog catalog,
        AiPersonality personality,
        IReadOnlyCollection<City> cities
    )
    {
        var constructionOptions = new List<AiConstructionOption>();

        foreach (var city in cities)
        {
            var buildings = city.GetConstructableBuildings(catalog.Templates);

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