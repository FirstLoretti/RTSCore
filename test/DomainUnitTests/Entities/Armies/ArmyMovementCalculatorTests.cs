using FluentAssertions;

using RTSCore.Domain.Entities.Armies;

namespace DomainUnitTests.Entities.Armies;

public class ArmyMovementCalculatorTests
{
    [Theory]
    [InlineData(0f, 0f, 0f, 0f, 10, 0)]
    [InlineData(0f, 0f, 3f, 4f, 10, 50)]
    public void CalculateCost_ShouldReturnExpectedValue(
        float posX, float posY,
        float destX, float destY,
        int unitDistanceCost,
        int expectedCost
    )
    {
        var calculatedCost = ArmyMovementCalculator.CalculateCost(new(posX, posY), new(destX, destY), unitDistanceCost);

        calculatedCost.Should().Be(expectedCost);
    }
}