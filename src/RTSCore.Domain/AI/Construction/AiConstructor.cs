using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.AI.Construction;

public static class AiConstructor
{
    public static void StartConstruction(
        Faction faction,
        AiPersonality personality,
        BuildingCatalog catalog,
        IReadOnlyCollection<City> cities
    )
    {
        var sortedOptions = EvaluateAvailableBuildings(catalog, personality, cities);

        foreach (var option in sortedOptions)
        {
            if (faction.Gold < option.Cost) continue;

            var city = cities.Single(c => c.Id == option.CityId);
            var buildingTemplate = catalog.GetBuilding(option.BuildingType);

            if (city.StartConstruction(buildingTemplate).IsFailure) continue;
            faction.SpendGold(option.Cost);
        }
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