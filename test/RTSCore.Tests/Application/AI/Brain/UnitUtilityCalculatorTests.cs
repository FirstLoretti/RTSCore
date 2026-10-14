using RTSCore.Application.AI.Brains;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.ValueObjects.AI;

namespace RTSCore.Tests.Application.AI.Brain;

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
            AiPersonalityType.Defender,
            new AiBudgetWeights(1, 1),
            new AiUnitWeights(1.25f, 0.75f),
            new AiBuildingWeights(EconomicMultiplier: 1, MilitaryMultiplier: 1),
            new AiDiplomacyWeights(1, 1, 1, 1, 1, 1, 1, 1, 1)
        );

        return (templates, aiPersonality);
    }
}