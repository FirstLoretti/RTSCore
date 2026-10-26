using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace DomainUnitTests.Entities.Cities;

public class BuildingTests
{
    public class Factory
    {
        [Fact]
        public void CreateUnderConstruction_ShouldCreateNotReady()
        {
            var template = TestData.CreateBuildingTemplate() with { TurnsToConstruct = 1 };
            var building = Building.CreateUnderConstruction(CampaignId.New(), template, FactionType.England, CityId.Empty);

            building.TurnsToConstruct.Should().Be(template.TurnsToConstruct);
            building.IsConstructed.Should().Be(false);
        }

        [Fact]
        public void CreateConstructed_ShouldCreateReady()
        {

            var template = TestData.CreateBuildingTemplate() with { TurnsToConstruct = 1 };
            var building = Building.CreateConstructed(CampaignId.New(), template, FactionType.England, CityId.Empty);

            building.TurnsToConstruct.Should().Be(0);
            building.IsConstructed.Should().Be(true);
        }
    }

    public class AdvanceConstruction
    {
        [Theory]
        [InlineData(2, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(0, 0, true)]
        public void ShouldReturnExpectedValues(int turnsToConstruct, int expectedTurns, bool isConstructed)
        {
            var template = TestData.CreateBuildingTemplate() with { TurnsToConstruct = turnsToConstruct };
            var building = TestData.CreateBuilding(template);

            building.AdvanceConstruction();

            building.IsConstructed.Should().Be(isConstructed);
            building.TurnsToConstruct.Should().Be(expectedTurns);
        }
    }
}