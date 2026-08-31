using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public static partial class GameBalance
{
    public static class Population
    {
        public const float BaseGrowthRate = 0.04f;

        public static float CalculateGrowthRate(City city)
        {
            float buildingsBonus = city.Buildings
                .Where(b => b.IsConstructed)
                .Select(b => Buildings.GetTemplate(b.Type))
                .SelectMany(t => t.Effects)
                .Where(e => e.Type == BuildingEffectType.PopulationGrowth)
                .Sum(e => e.Value);

            float finalGrowthrate = BaseGrowthRate + buildingsBonus;

            return finalGrowthrate;
        }
    }
}