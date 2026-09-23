using RTSCore.Application.AI.Brains;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.AI;
using RTSCore.Domain.ValueObjects.Presets;

namespace RTSCore.Tests.Application.AI.Brain;

public class BuildingUtilityCalculatorTests
{
    [Fact]
    public void GetOrderedOptions_AgressiveAi_ShouldPrioritizeMilitaryOption()
    {
        var (calculator, cities) = Arrange(isDifferentFaction: false);

        var options = calculator.GetOrderedOptions(cities);

        Assert.Equal(4, options.Count);
        Assert.Equal(BuildingType.ReqruitBarrack, options[0].BuildingType);
        Assert.Equal(BuildingType.Market, options[3].BuildingType);
        Assert.True(options[0].Score > options[3].Score);
    }

    [Fact]
    public void GetOrderedOptions_CitiesWithDifferentFactions_ShouldThrow()
    {
        var (calculator, cities) = Arrange(isDifferentFaction: true);

        Assert.Throws<InvalidOperationException>(() => calculator.GetOrderedOptions(cities));
    }

    private static (BuildingUtilityCalculator, City[]) Arrange(bool isDifferentFaction)
    {
        var templates = new BuildingTemplate[]
        {
            new(BuildingType.Market, "Market", 1000, 1, [CityType.Village], BuildingCategory.Economic, 50),
            new(BuildingType.ReqruitBarrack, "Barrack", 1000, 1, [CityType.Village], BuildingCategory.Military, 50)
        };

        var aiPersonality = new AiPersonality(
            AiStrategicType.Aggressive,
            new BudgetWeights(1, 1),
            new UnitWeights(1, 1),
            new BuildingWeights(EconomicMultiplier: 0.75f, MilitaryMultiplier: 1.25f),
            new DiplomacyWeights(1, 1, 1, 1, 1, 1, 1, 1, 1)
        );

        var cityPreset = new CityPreset("id", "City", CityType.Village, 1, []);
        var cityPreset2 = new CityPreset("id2", "City2", CityType.Village, 1, []);
        City[] cities = isDifferentFaction
            ? [new(cityPreset, FactionType.England), new(cityPreset2, FactionType.France)]
            : [new(cityPreset, FactionType.England), new(cityPreset2, FactionType.England)];



        var calculator = new BuildingUtilityCalculator(templates, aiPersonality, new FakeCityBuildingRegistry());

        return (calculator, cities);
    }
}

public class FakeCityBuildingRegistry : ICityBuildingRegistry
{
    public IReadOnlyCollection<BuildingType> GetBuildingOptions(CityType type)
    {
        return [BuildingType.Market, BuildingType.ReqruitBarrack];
    }
}