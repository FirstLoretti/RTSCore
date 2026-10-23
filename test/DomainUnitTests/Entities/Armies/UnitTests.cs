using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;

namespace DomainUnitTests.Entities.Armies;

public class UnitTests
{
    public class Factory
    {
        [Fact]
        public void CreateReady_ShouldBeAliveAndRecruited()
        {
            var template = TestData.CreateUnitTemplate(maxHealth: 100);

            var unit = Unit.CreateReady(CampaignId.New(), FactionType.England, template, ArmyId.New());

            unit.IsAlive.Should().BeTrue();
            unit.IsRecruited.Should().BeTrue();
        }

        [Fact]
        public void CreateTraining_ShouldBeAliveAndNotRecruited()
        {
            var template = TestData.CreateUnitTemplate(maxHealth: 100);

            var unit = Unit.CreateTraining(CampaignId.New(), FactionType.England, template, ArmyId.New());

            unit.IsAlive.Should().BeTrue();
            unit.IsRecruited.Should().BeFalse();
        }
    }

    public class TakeDamage
    {
        [Theory]
        [InlineData(99, 1, true)]
        [InlineData(200, 0, false)]
        public void ShouldReturnExpectedValue(int amount, int expectedHealth, bool isAlive)
        {
            var unit = TestData.CreateUnit(TestData.CreateUnitTemplate(maxHealth: 100));

            unit.TakeDamage(amount);

            unit.Health.Should().Be(expectedHealth);
            unit.IsAlive.Should().Be(isAlive);
        }
    }

    public class AdvanceRecruitment
    {
        [Theory]
        [InlineData(2, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(0, 0, true)]
        public void ShouldReturnExpectedValue(int turnsToRecruit, int expectedTurns, bool isRecruited)
        {
            var template = TestData.CreateUnitTemplate(maxHealth: 100) with { TurnsToRecruit = turnsToRecruit };
            var unit = TestData.CreateUnit(template);

            unit.AdvanceRecruitment();

            unit.TurnsToRecruit.Should().Be(expectedTurns);
            unit.IsRecruited.Should().Be(isRecruited);
        }
    }

    public class AddExperience
    {
        [Theory]
        [InlineData(5, 0)]
        [InlineData(11, 1)]
        [InlineData(30, 3)]
        [InlineData(31, 3)]
        public void ShouldReturnExpectedValue(int amount, int expectedLevel)
        {
            var expToLevel = new int[] { 0, 10, 20, 30 };
            var template = TestData.CreateUnitTemplate(maxHealth: 100) with { TurnsToRecruit = 0 };
            var unit = TestData.CreateUnit(template);

            unit.AddExperience(amount, expToLevel);

            unit.Level.Should().Be(expectedLevel);
            unit.Experience.Should().Be(amount);
        }
    }
}