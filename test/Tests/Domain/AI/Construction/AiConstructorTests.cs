using FluentAssertions;

using RTSCore.Domain.AI.Construction;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;

using static Tests.TestData;

namespace Tests.Domain.AI.Construction;

public class AiConstructorTests
{
    [Fact]
    public void StartConstruction_ShouldConstructOnlyHighestUtilityBuilding()
    {
        var faction = CreateFaction();
        var initialGold = faction.Gold;
        var buildingCost = (int)(faction.Gold * AiBudgetWeights.Construction);
        var city = CreateCity(template: CreateCityTemplate());
        var building1 = CreateBuildingTemplate() with { Type = BuildingType.ReqruitBarrack, AiUtility = 1 };
        var building2 = CreateBuildingTemplate() with
        {
            Type = BuildingType.Market,
            Cost = buildingCost,
            AiUtility = 2
        };
        var catalog = new BuildingCatalog([building1, building2]);
        var aiConstructor = new AiConstructor(TestData.AiCatalog, catalog, AiBudgetWeights);

        aiConstructor.StartConstruction(faction, AiPersonalityType.Conqueror, [city]);

        faction.Gold.Should().Be(initialGold - buildingCost); ;
        city.Buildings.Should().ContainSingle(b => b.Cost == building2.Cost);
    }

    [Fact]
    public void StartConstruction_ShouldSkipHighestUtilityBuildingWhenGoldNotEnough()
    {
        var faction = CreateFaction();
        var city1 = CreateCity(template: CreateCityTemplate());
        var city2 = CreateCity(template: CreateCityTemplate());
        var building1 = CreateBuildingTemplate() with { Type = BuildingType.ReqruitBarrack, AiUtility = 1 };
        var building2 = CreateBuildingTemplate() with { Type = BuildingType.Market, Cost = faction.Gold + 1, AiUtility = 2 };
        var catalog = new BuildingCatalog([building1, building2]);
        var expectedGold = faction.Gold - building1.Cost * 2;
        var aiConstructor = new AiConstructor(TestData.AiCatalog, catalog, AiBudgetWeights);

        aiConstructor.StartConstruction(faction, AiPersonalityType.Conqueror, [city1, city2]);

        faction.Gold.Should().Be(expectedGold);
        city1.Buildings.Should().ContainSingle(b => b.Cost == building1.Cost);
        city2.Buildings.Should().ContainSingle(b => b.Cost == building1.Cost);
    }
}