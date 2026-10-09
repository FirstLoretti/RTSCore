using RTSCore.Application.Common.Validation;
using RTSCore.Domain.AI.Services;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.AI.Construction;

public class AiConstructor(
    IUnitOfWork unitOfWork,
    AiBuildingUtilityCalculator aiUtilityCalculator,
    AiConstructionService constructionService,
    BuildingConfiguration buildingConfiguration
)
{
    public async Task HandleConstructLogicAsync(FactionType factionType, CancellationToken ct)
    {
        var cities = await unitOfWork.CityRepository.GetCitiesAsync(factionType, ct);
        var faction = await unitOfWork.FactionRepository.GetFactionAsync(factionType, ct);
        Guard.Against.NotFound(faction, factionType);

        var constructionOptions = aiUtilityCalculator.GetOrderedConstructionOptions(cities);
        var buildingsToConstruct = constructionService.SimulateConstruction(constructionOptions, faction.Gold);

        foreach (var building in buildingsToConstruct)
        {
            var city = cities.FirstOrDefault(c => c.Id == building.CityId);
            Guard.Against.NotFound(city, building.CityId);

            var buildingTemplate = buildingConfiguration.Templates.First(t => t.Type == building.BuildingType);

            city.StartConstruction(buildingTemplate);
            faction.SpendGold(building.Cost);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}