using RTSCore.Domain.Services;

namespace RTSCore.Tests;

public class GameBalanceTests
{
    [Theory]
    [InlineData(25f, 1.1f, 2, 28)]
    public void CalculateStats_ShouldRoundUp_WhenValueIsFractional(
        float statValue,
        float growthRate,
        int level,
        int expected
    )
    {
        int value = Units.CalculateStat(statValue, growthRate, level);

        Assert.Equal(expected, value);
    }
}