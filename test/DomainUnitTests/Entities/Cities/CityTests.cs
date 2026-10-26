using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Errors;

namespace DomainUnitTests.Entities.Cities;

public class CityTests
{
    public class GetRecruitableUnits
    {
        [Fact]
        public void ShouldReturnAvailableProductionOptions()
        {

            var (unitTemplates, reqruitBarrack, militiaBarrack, city) = PrepareTestData();

            city.StartConstruction(reqruitBarrack);
            city.StartConstruction(militiaBarrack);

            var options = city.GetRecruitableUnits(unitTemplates);

            options.Single().DisplayName.Should().Be(unitTemplates[0].DisplayName);
        }

        private static (
            UnitTemplate[] unitTemplates,
            BuildingTemplate reqruitBarrack,
            BuildingTemplate militiaBarrack,
            City city
        ) PrepareTestData()
        {
            UnitTemplate[] unitTemplates =
            [
                TestData.CreateUnitTemplate(maxHealth: 100) with
                {
                    RequiredBuilding = BuildingType.ReqruitBarrack, DisplayName = "1"
                },
                TestData.CreateUnitTemplate(maxHealth: 100) with {RequiredBuilding = BuildingType.MilitiaBarrack},
                TestData.CreateUnitTemplate(maxHealth: 100) with {RequiredBuilding = BuildingType.Market},
            ];
            var reqruitBarrack = TestData.CreateBuildingTemplate() with
            {
                Type = BuildingType.ReqruitBarrack,
                TurnsToConstruct = 0
            };
            var militiaBarrack = TestData.CreateBuildingTemplate() with
            {
                Type = BuildingType.MilitiaBarrack,
                TurnsToConstruct = 1
            };
            var cityTemplate = TestData.CreateCityTemplate() with
            {
                ConstructableBuildings = [BuildingType.ReqruitBarrack, BuildingType.MilitiaBarrack, BuildingType.Market]
            };
            var city = TestData.CreateCity(cityTemplate);

            return (unitTemplates, reqruitBarrack, militiaBarrack, city);
        }
    }

    public class GetConstructableBuildings()
    {
        [Fact]
        public void ShouldReturnAvailableProductionOptions()
        {
            var city = TestData.CreateCity(TestData.CreateCityTemplate());

            city.ConstructInstally(TestData.BuildingCatalog.GetBuilding(BuildingType.ReqruitBarrack));

            var options = city.GetConstructableBuildings(TestData.BuildingCatalog.Templates);

            options.Select(o => o.Type).Should().NotContain([BuildingType.ReqruitBarrack, BuildingType.CultivatedFieldsComplex]);
            options.Select(o => o.Type).Should().Contain([BuildingType.MilitiaBarrack, BuildingType.CultivatedField]);
        }
    }

    public class CreateArmy()
    {
        [Fact]
        public void ShouldCreateCorrect()
        {
            var city = TestData.CreateCity(TestData.CreateCityTemplate());

            var army = city.CreateArmy(TestData.CreateUnitTemplate(maxHealth: 100));

            army.CampaignId.Should().Be(city.CampaignId);
            army.Faction.Should().Be(city.Faction);
        }
    }

    public class TurnEnd
    {
        [Fact]
        public void Should_GrowPopulation_CalculateIncome_AdvanceConstruction_AndRaiseTurnEndEvent()
        {
            var template = TestData.CreateCityTemplate() with { StartingPopulation = 1000 };
            var city = TestData.CreateCity(template);
            var market = TestData.BuildingCatalog.GetBuilding(BuildingType.Market);
            var bigMarket = TestData.BuildingCatalog.GetBuilding(BuildingType.BigMarket);
            var field = TestData.BuildingCatalog.GetBuilding(BuildingType.CultivatedField);
            var fieldComplex = TestData.BuildingCatalog.GetBuilding(BuildingType.CultivatedFieldsComplex) with { TurnsToConstruct = 2 };

            city.ConstructInstally(market);
            city.ConstructInstally(field);
            city.StartConstruction(bigMarket);
            city.StartConstruction(fieldComplex);

            city.TurnEnd(TestData.BuildingCatalog, TestData.CityConfiguration);

            var turnEvent = city.DomainEvents
                .OfType<CityTurnEndEvent>()
                .Should()
                .ContainSingle()
                .Subject;

            var fieldGrowth = field.Effects.Single(e => e.Type == BuildingEffectType.PopulationGrowth).Value;
            city.Population.Should().Be((int)(template.StartingPopulation + template.StartingPopulation * fieldGrowth));

            var marketIncome = (int)market.Effects.Single(e => e.Type == BuildingEffectType.GoldIncome).Value;
            var bigMarketIncome = (int)bigMarket.Effects.Single(e => e.Type == BuildingEffectType.GoldIncome).Value;
            turnEvent.BuildingsIncome.Should().Be(marketIncome + bigMarketIncome);

            turnEvent.TaxesIncome.Should().Be((int)(city.Population * TestData.CityConfiguration.TaxPerCitizen));

            city.Buildings.Single(b => b.Type == BuildingType.BigMarket).IsConstructed.Should().BeTrue();
            city.Buildings.Single(b => b.Type == BuildingType.CultivatedFieldsComplex).TurnsToConstruct.Should().Be(1);
        }
    }

    public class CanConstruct
    {
        [Fact]
        public void ShouldFailWhenBuildingTypeNotAllowedInCity()
        {
            var cityTemplate = TestData.CreateCityTemplate() with { Type = CityType.Village };
            var buildingTemplate = TestData.CreateBuildingTemplate() with { AllowedCityTypes = [CityType.Settlement] };
            var city = TestData.CreateCity(cityTemplate);

            var result = city.CanConstruct(buildingTemplate);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CityCantConstructThisBuildingType);
        }

        [Fact]
        public void ShouldFailWhenBuildingTypeAlreadyAdded()
        {
            var city = TestData.CreateCity(TestData.CreateCityTemplate());
            var building = TestData.CreateBuildingTemplate() with { Type = BuildingType.Market };
            city.StartConstruction(building);

            var result = city.CanConstruct(building);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantConstructBuildingDuplicate);
        }

        [Fact]
        public void ShouldFailWhenChainRulesNotMet()
        {
            var city = TestData.CreateCity(TestData.CreateCityTemplate());
            var building = TestData.CreateBuildingTemplate() with { RequiredBuildings = [BuildingType.Market] };

            var result = city.CanConstruct(building);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.ChainUnavailableConstructionOption);
        }
    }

    public class StartConstruction()
    {
        [Fact]
        public void ShouldConstructWhenAllConditionsMet()
        {
            var template = TestData.CreateBuildingTemplate() with { Type = BuildingType.Market, RequiredBuildings = [] };
            var city = TestData.CreateCity(TestData.CreateCityTemplate());

            var result = city.StartConstruction(template);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(BuildingId.Empty);
            city.Buildings.Should().Contain(b => b.Type == template.Type && !b.IsConstructed);
        }
    }

    public class ConstructInstally()
    {
        [Fact]
        public void ShouldConstructWhenAllConditionsMet()
        {
            var template = TestData.CreateBuildingTemplate() with { Type = BuildingType.Market, RequiredBuildings = [] };
            var city = TestData.CreateCity(TestData.CreateCityTemplate());

            var result = city.ConstructInstally(template);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(BuildingId.Empty);
            city.Buildings.Should().Contain(b => b.Type == template.Type && b.IsConstructed);
        }
    }

    public class CancelConstruction
    {
        [Fact]
        public void ShouldFailIfBuildingConstructed()
        {
            var city = TestData.CreateCity(TestData.CreateCityTemplate());
            var buildingId = city.ConstructInstally(TestData.CreateBuildingTemplate()).Value;

            var result = city.CancelConstruction(buildingId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantCancelAlreadyConstructed);
            city.Buildings.Should().Contain(b => b.Id == buildingId);
        }

        [Fact]
        public void ShouldCancelWhenAllConditionsMet()
        {
            var city = TestData.CreateCity(TestData.CreateCityTemplate());
            var buildingId = city.StartConstruction(TestData.CreateBuildingTemplate()).Value;

            var result = city.CancelConstruction(buildingId);

            result.IsSuccess.Should().BeTrue();
            city.Buildings.Should().NotContain(b => b.Id == buildingId);
        }
    }
}