using RTSCore.Domain.AI.Economy;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Configurations.Buldings;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Application.AI.Construction;

public sealed class AiConstructor(IUnitOfWork unitOfWork, BuildingCatalog buildingCatalog, AiCatalog aiCatalog)
{
    public async Task HandleConstructLogicAsync(FactionType factionType, CancellationToken ct)
    {
        var cities = await unitOfWork.CityRepository.GetAsync(factionType, ct);
        var faction = await unitOfWork.FactionRepository.GetAsync(factionType, ct);
        Guard.NotFound(faction, factionType);

        var personality = aiCatalog.GetPersonality(faction.AiPersonalityType);
        var constructionOptions = AiBuildingUtilityCalculator.OrderConstructionOptions(buildingCatalog, personality, cities);
        var buildingsToConstruct = AiConstructionSimulator.Simulate(constructionOptions, faction.Gold);

        StartConstruction(faction, cities, buildingsToConstruct);

        await unitOfWork.SaveChangesAsync(ct);
    }

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