using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public static class CityEconomyCalculator
{
    public static float CalculateCityGrowthRate(
        City city,
        IReadOnlyDictionary<BuildingType, BuildingTemplate> buildings,
        IReadOnlyDictionary<CityType, CityTemplate> cities
    )
    {
        if (!cities.TryGetValue(city.Type, out var cityTemplate))
            throw new InvalidOperationException("Шаблон города не добавлен");

        float buildingsBonus = 0f;
        foreach (var building in city.Buildings.Where(b => b.IsConstructed))
        {
            if (!buildings.TryGetValue(building.Type, out var template))
                throw new InvalidOperationException("Коллекция не содержит шаблон");

            if (template.Effects != null)
            {
                buildingsBonus += template.Effects
                    .FirstOrDefault(e => e.Type == BuildingEffectType.PopulationGrowth)
                    .Value;
            }
        }

        return buildingsBonus + cityTemplate.GrowthRate;
    }

    public static int CalculateBuildingsIncome(
        City city,
        IReadOnlyDictionary<BuildingType, BuildingTemplate> buildings
    )
    {
        float income = 0f;
        foreach (var building in city.Buildings.Where(b => b.IsConstructed))
        {
            if (!buildings.TryGetValue(building.Type, out var template))
                throw new InvalidOperationException("Коллекция не содержит шаблон");

            if (template.Effects != null)
            {
                income += template.Effects
                    .FirstOrDefault(e => e.Type == BuildingEffectType.GoldIncome)
                    .Value;
            }
        }

        return (int)income;
    }

    public static int CalculateTaxIncome(CityTemplate template, int population)
        => (int)(population * template.TaxRatePerCitizen);
}