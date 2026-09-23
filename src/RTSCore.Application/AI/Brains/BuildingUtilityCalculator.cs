using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.AI;

namespace RTSCore.Application.AI.Brains;

public class BuildingUtilityCalculator(
    IReadOnlyCollection<BuildingTemplate> buildingTemplates,
    AiPersonality personality,
    ICityBuildingRegistry buildingRegistry
)
{
    public List<BuildingOptionScore> GetOrderedOptions(IReadOnlyCollection<City> cities)
    {
        if (cities == null)
        {
            throw new ArgumentNullException(nameof(cities), "Коллекция городов не может быть null");
        }

        var firstCity = cities.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"[{nameof(BuildingUtilityCalculator)}] Получена пустая коллекция городов"
            );

        var isSameFactions = cities.All(c => c.OwnerFaction == firstCity.OwnerFaction);

        if (!isSameFactions)
        {
            throw new InvalidOperationException(
                $"[{nameof(BuildingUtilityCalculator)}] Получена коллекция с городами разных фракций"
            );
        }

        var bildingOptions = new List<BuildingOptionScore>();

        foreach (var city in cities)
        {
            var options = buildingRegistry.GetBuildingOptions(city.Type);
            var availableOptions = city.GetAvailableConstructOptions(options);

            foreach (var type in availableOptions)
            {
                var template = buildingTemplates.FirstOrDefault(t => t.Type == type)
                    ?? throw new InvalidOperationException(
                        $"[{nameof(BuildingUtilityCalculator)}] Шаблон здания {type} не найден"
                    );

                var multiplier = template.Category == BuildingCategory.Military
                    ? personality.BuildingWeights.MilitaryMultiplier
                    : personality.BuildingWeights.EconomicMultiplier;

                var score = (int)(template.AiUtility * multiplier);

                if (score > 0)
                {
                    var constructOption = new BuildingOptionScore(city.Id, type, template.Cost, score);
                    bildingOptions.Add(constructOption);
                }
            }
        }

        return [.. bildingOptions.OrderByDescending(o => o.Score)];
    }
}