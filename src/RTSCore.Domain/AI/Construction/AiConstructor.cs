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
        var buildingsToConstruct = PlaneConstruction(catalog, personality, cities, faction.Gold);

        foreach (var building in buildingsToConstruct)
        {
            var city = cities.Single(c => c.Id == building.CityId);
            var buildingTemplate = catalog.GetBuilding(building.BuildingType);

            faction.SpendGold(building.Cost);
            city.StartConstruction(buildingTemplate);
        }
    }

    private static List<AiConstructionOption> PlaneConstruction(
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