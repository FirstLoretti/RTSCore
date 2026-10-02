using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

internal static class CityEconomyCalculator
{
    public static float CalculateGrowthRate(
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

    public static int CalculateTurnEndIncome(
        CityTemplate template,
        int population,
        IReadOnlyList<BuildingTemplate> constructedTemplates
    )
    {
        var buildingsIncome = CalculateBuildingsIncome(constructedTemplates);
        var taxIncome = CalculateTaxIncome(template, population);

        return buildingsIncome + taxIncome;
    }

    private static int CalculateBuildingsIncome(
        IReadOnlyList<BuildingTemplate> constructedTemplates
    )
    {
        int income = 0;
        for (int i = 0; i < constructedTemplates.Count; i++)
        {
            income += constructedTemplates[i].Effects
                .FirstOrDefault(e => e.Type == BuildingEffectType.GoldIncome)
                .Value;
        }

        return income;
    }

    private static int CalculateTaxIncome(CityTemplate template, int population)
        => (int)(population * template.TaxRatePerCitizen);
}