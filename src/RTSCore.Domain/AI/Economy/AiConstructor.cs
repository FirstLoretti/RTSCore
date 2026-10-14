using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.AI.Economy;

public sealed class AiConstructor(BuildingCatalog catalog)
{
    private void StartConstruction(
        Faction faction,
        AiPersonality personality,
        IReadOnlyCollection<City> cities
    )
    {
        var buildingsToConstruct = AiConstructionPlanner.PlaneConstruction(catalog, personality, cities, faction.Gold);

        foreach (var building in buildingsToConstruct)
        {
            var city = cities.First(c => c.Id == building.CityId);
            var buildingTemplate = catalog.Get(building.BuildingType);

            faction.SpendGold(building.Cost);
            city.StartConstruction(buildingTemplate);
        }
    }
}