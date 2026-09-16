using RTSCore.Application.Campaign.Services.AiRecruiter;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.AI;

namespace RTSCore.Tests.UnitTests.Application.Ai;

public class UnitUtilityCalculatorTests
{
    [Fact]
    public void CalculateFor_AgressiveAi_ShouldPrioritizeMilitaryUnit()
    {
        var (templates, personality) = Arrange(isWrongUnitType: false);

        var calculator = new UnitUtilityCalculator();
        var options = calculator.CalculateFor(templates, personality);

        Assert.Equal(2, options.Count);
        Assert.Equal(UnitType.Peasant, options.ElementAt(0).Unit);
        Assert.Equal(UnitType.PeasantArcher, options.ElementAt(1).Unit);
        Assert.True(options.ElementAt(0).Utility > options.ElementAt(1).Utility);
    }

    [Fact]
    public void CalculateFor_WrongUnitType_ShouldThrow()
    {
        var (templates, personality) = Arrange(isWrongUnitType: true);

        var calculator = new UnitUtilityCalculator();

        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculateFor(templates, personality));
    }

    private static (UnitTemplate[], AiPersonality) Arrange(bool isWrongUnitType)
    {
        var templates = isWrongUnitType
            ? [new(UnitType.Peasant, "Range", 1, 1, 1, 1, 1, 1, 1, 1, 1, UnitCategory.None, 50),]
            : new UnitTemplate[]{
                new(UnitType.PeasantArcher, "Range", 1, 1, 1, 1, 1, 1, 1, 1, 1, UnitCategory.RangeInfantry, 50),
                new(UnitType.Peasant, "Melee", 1, 1, 1, 1, 1, 1, 1, 1, 1, UnitCategory.Infantry, 50)};

        var aiPersonality = new AiPersonality(
            AiStrategicType.Aggressive,
            new BudgetWeights(1, 1),
            new UnitWeights(1.25f, 0.75f),
            new BuildingWeights(EconomicMultiplier: 1, MilitaryMultiplier: 1),
            new DiplomacyWeights(1, 1, 1, 1, 1, 1, 1, 1, 1)
        );

        return (templates, aiPersonality);
    }
}