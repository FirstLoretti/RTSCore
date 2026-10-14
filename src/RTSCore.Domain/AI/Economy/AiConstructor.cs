using RTSCore.Domain;
using RTSCore.Domain.AI.Economy;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Application.AI.Construction;

public sealed class AiConstructor(BuildingCatalog buildingCatalog)
{

    private void StartConstruction(
        Faction faction,
        IReadOnlyCollection<City> cities,
        IReadOnlyCollection<AiConstructionOption> buildingsToConstruct
    )
    {
        foreach (var building in buildingsToConstruct)
        {
            var city = cities.First(c => c.Id == building.CityId);
            var buildingTemplate = buildingCatalog.Get(building.BuildingType);

            city.StartConstruction(buildingTemplate);
            faction.SpendGold(building.Cost);
        }
    }
}