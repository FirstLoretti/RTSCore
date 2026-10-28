using System.Numerics;

using FluentAssertions;

using RTSCore.Domain.AI.Construction;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;

using static DomainUnitTests.TestData;

namespace DomainUnitTests.AI;

public class AiConstructorTests
{
    [Fact]
    public void StartConstruction_ShouldConstructOnlyHighestUtilityBuilding()
    {
        var faction = CreateFaction();
        var city = CreateCity(CreateCityTemplate());
        var building1 = CreateBuildingTemplate() with { Type = BuildingType.ReqruitBarrack, AiUtility = 1 };
        var building2 = CreateBuildingTemplate() with { Type = BuildingType.Market, Cost = faction.Gold, AiUtility = 2 };
        var catalog = new BuildingCatalog([building1, building2]);

        AiConstructor.StartConstruction(faction, AiPersonality, catalog, [city]);

        faction.Gold.Should().Be(0);
        city.Buildings.Should().ContainSingle(b => b.Cost == building2.Cost);
    }

    [Fact]
    public void StartConstruction_ShouldSkipHighestUtilityBuildingWhenGoldNotEnough()
    {
        var faction = CreateFaction();
        var city1 = CreateCity(CreateCityTemplate());
        var city2 = CreateCity(CreateCityTemplate());
        var building1 = CreateBuildingTemplate() with { Type = BuildingType.ReqruitBarrack, AiUtility = 1 };
        var building2 = CreateBuildingTemplate() with { Type = BuildingType.Market, Cost = faction.Gold + 1, AiUtility = 2 };
        var catalog = new BuildingCatalog([building1, building2]);
        var expectedGold = faction.Gold - building1.Cost * 2;

        AiConstructor.StartConstruction(faction, AiPersonality, catalog, [city1, city2]);

        faction.Gold.Should().Be(expectedGold);
        city1.Buildings.Should().ContainSingle(b => b.Cost == building1.Cost);
        city2.Buildings.Should().ContainSingle(b => b.Cost == building1.Cost);
    }
}